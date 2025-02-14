using IdGen;
using Npgsql;
using Storoey.Database.PostgreSQL.Extensions;
using Storoey.Database.PostgreSQL.Models;
using Storoey.Database.PostgreSQL.Options;
using Storoey.Database.PostgreSQL.Parameters;

namespace Storoey.Database.PostgreSQL;

/// <summary>
///     Represents a client for interacting with a PostgreSQL database using specified options.
///     Provides methods for performing CRUD operations as well as managing connections and transactions.
/// </summary>
public class Client(ClientOptions clientOptions) : IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(
        $"Host={clientOptions.Host};Port={clientOptions.Port};Database={clientOptions.Database};Username={clientOptions.Username};Password={clientOptions.Password};");

    private readonly IdGenerator _snowflakeIdGenerator = new(clientOptions.MachineId);

    public NpgsqlConnection? Connection { get; private set; }

    /// <summary>
    ///     Disposes of resources used by the client asynchronously, including closing connections and disposing
    ///     of the underlying PostgreSQL data source.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await Close();
        await _dataSource.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Generates the next unique identity value using a snowflake ID generator.
    /// </summary>
    /// <returns>The next unique identity value as a long integer.</returns>
    public long NextIdentity()
    {
        return _snowflakeIdGenerator.CreateId();
    }

    /// <summary>
    ///     Establishes a connection to the PostgreSQL database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown if the connection could not be established.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Connect(CancellationToken cancellationToken = default)
    {
        Connection ??= await _dataSource.OpenConnectionAsync(cancellationToken);

        if (Connection is null)
        {
            throw new InvalidOperationException("Connection could not be established.");
        }
    }

    /// <summary>
    ///     Closes the connection to the PostgreSQL database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Close()
    {
        if (Connection is null)
        {
            return;
        }

        await Connection.CloseAsync();
        await Connection.DisposeAsync();
        Connection = null;
    }
    
    /// <summary>
    ///     Inserts a row into the specified PostgreSQL table.
    /// </summary>
    /// <param name="parameters">The parameter object containing the table name, column names, and data to be inserted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when the connection to the database cannot be established.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Insert(InsertParameter parameters, CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        await using var command = Connection!.CreateCommand();
        command.CommandText = parameters.CommandText;
        command.Parameters.AddRange(parameters.Values.Select(parameter => parameter.ToNpgsqlParameter()).ToArray());
        command.Transaction = parameters.Transaction;

        await command.PrepareAsync(cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    
    public async Task InsertRaw(string commandText, NpgsqlParameter[] parameters, NpgsqlTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        await using var command = Connection!.CreateCommand();
        command.CommandText = commandText;
        command.Parameters.AddRange(parameters);
        command.Transaction = transaction;

        await command.PrepareAsync(cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> Insert<T>(InsertParameter parameters, CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        await using var command = Connection!.CreateCommand();
        command.CommandText = parameters.CommandText;
        command.Parameters.AddRange(parameters.Values.Select(parameter => parameter.ToNpgsqlParameter()).ToArray());
        command.Transaction = parameters.Transaction;

        await command.PrepareAsync(cancellationToken);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        
        return result is T column ? column : default;
    }

    /// <summary>
    ///     Inserts a batch of rows into the specified PostgreSQL table using binary import for high performance.
    /// </summary>
    /// <param name="parameters">The parameter object containing the table name, column names, and data to be inserted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when the connection to the database cannot be established.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InsertBatch(InsertBatchParameter parameters,
        CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        var commandText = $"COPY {parameters.Table} ({string.Join(", ", parameters.Columns)}) FROM STDIN BINARY";

        await using var writer = await Connection!.BeginBinaryImportAsync(commandText, cancellationToken);

        foreach (var parameterList in parameters.Values)
        {
            await writer.StartRowAsync(cancellationToken);

            foreach (var parameter in parameterList)
            {
                if (parameter is DBNull)
                {
                    await writer.WriteNullAsync(cancellationToken);
                    continue;
                }

                if (parameter.TryToNpgsqlDbType(out var npgsqlDbType))
                {
                    await writer.WriteAsync(parameter, npgsqlDbType, cancellationToken);
                    continue;
                }

                await writer.WriteAsync(parameter, cancellationToken);
            }
        }

        await writer.CompleteAsync(cancellationToken);
    }

    /// <summary>
    ///     Executes a SQL query and retrieves the first row or null if no rows match the query.
    /// </summary>
    /// <param name="parameters">
    ///     The parameters required for executing the query, including the SQL command text, query
    ///     parameters, and optional transaction information.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the first row of the query result
    ///     or null if no rows are found.
    /// </returns>
    public async Task<TableRow?> FirstOrDefault(WhereParameter parameters,
        CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        await using var command = Connection!.CreateCommand();
        command.CommandText = parameters.CommandText.Contains("LIMIT")
            ? parameters.CommandText
            : parameters.CommandText + " LIMIT 1";
        if (parameters.Parameters is not null)
        {
            command.Parameters.AddRange(parameters.Parameters.Select(parameter => parameter.ToNpgsqlParameter())
                .ToArray());
        }

        await command.PrepareAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var tableRow = new TableRow
        {
            Columns = new TableColumn[reader.FieldCount]
        };

        for (var fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
        {
            var columnType = reader.GetFieldType(fieldIndex);
            var columnName = reader.GetName(fieldIndex);
            var columnValue = reader.GetValue(fieldIndex);

            tableRow.Columns[fieldIndex] = new TableColumn
            {
                Name = columnName,
                Value = columnValue,
                Type = columnType
            };
        }

        return tableRow;
    }

    /// <summary>
    ///     Executes a WHERE query on the PostgreSQL database and retrieves matching rows.
    /// </summary>
    /// <param name="parameters">The WHERE query parameters, including the SQL command text and optional parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the rows matching the query.</returns>
    public async Task<TableRow[]> Where(WhereParameter parameters, CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        await using var command = Connection!.CreateCommand();
        command.CommandText = parameters.CommandText;
        if (parameters.Parameters is not null)
        {
            command.Parameters.AddRange(parameters.Parameters.Select(parameter => parameter.ToNpgsqlParameter())
                .ToArray());
        }

        await command.PrepareAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var tableRows = new List<TableRow>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var tableRow = new TableRow
            {
                Columns = new TableColumn[reader.FieldCount]
            };

            for (var fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
            {
                var columnType = reader.GetFieldType(fieldIndex);
                var columnName = reader.GetName(fieldIndex);
                var columnValue = reader.GetValue(fieldIndex);

                tableRow.Columns[fieldIndex] = new TableColumn
                {
                    Name = columnName,
                    Value = columnValue,
                    Type = columnType
                };
            }

            tableRows.Add(tableRow);
        }

        return tableRows.ToArray();
    }

    public async Task<T[]> Where<T>(MappedWhereParameter<T> parameters, CancellationToken cancellationToken = default)
        where T : new()
    {
        await Connect(cancellationToken);

        await using var command = Connection!.CreateCommand();
        command.CommandText = parameters.CommandText;
        if (parameters.Parameters is not null)
        {
            command.Parameters.AddRange(parameters.Parameters.Select(parameter => parameter.ToNpgsqlParameter())
                .ToArray());
        }

        await command.PrepareAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var tableRows = new List<T>();

        while (await reader.ReadAsync(cancellationToken))
        {
            tableRows.Add(parameters.Map(reader));
        }

        return tableRows.ToArray();
    }
}