using System.ComponentModel.DataAnnotations;
using Npgsql;

namespace Storoey.Database.PostgreSQL.Parameters;

/// <summary>
///     Represents the parameters used for executing an INSERT command in a PostgreSQL database.
/// </summary>
public sealed record InsertParameter
{
    /// <summary>
    ///     Gets the SQL command text to be executed in a PostgreSQL database.
    /// </summary>
    /// <remarks>
    ///     The command text typically represents a SQL query or statement (e.g., an INSERT or SELECT command).
    ///     This property is required and must be specified before execution.
    /// </remarks>
    [Required]
    public required string CommandText { get; init; }

    /// <summary>
    ///     Gets the values being used as parameters for executing an INSERT command in a PostgreSQL database.
    /// </summary>
    /// <remarks>
    ///     This collection represents the data to be inserted into the table. Each element corresponds to a value for a
    ///     specific column.
    ///     Ensure that the number and order of the values match the SQL command and table schema.
    ///     This property is required and must be specified before execution.
    /// </remarks>
    [Required]
    public required object[] Values { get; init; }

    /// <summary>
    ///     Gets or sets the database transaction to be used when executing the command.
    /// </summary>
    /// <remarks>
    ///     This property is optional and allows associating the command with an existing
    ///     transactional context. If not set, the command executes outside any transaction.
    ///     Use this property when you need to perform multiple database operations within
    ///     the same transactional scope.
    /// </remarks>
    public NpgsqlTransaction? Transaction { get; init; }
}