using System.ComponentModel.DataAnnotations;

namespace Storoey.Database.PostgreSQL.Parameters;

/// <summary>
///     Represents parameters required for batch insertion of rows into a PostgreSQL table.
/// </summary>
/// <remarks>
///     This class encapsulates the table name, parameter names (columns), and a matrix of parameter data for executing
///     a high-performance binary import operation using the COPY command in PostgreSQL.
/// </remarks>
/// <example>
///     Can be used to specify the table and data for inserting multiple rows at once.
/// </example>
public sealed record InsertBatchParameter
{
    /// <summary>
    ///     Represents the name of the PostgreSQL table targeted for batch data insertion.
    /// </summary>
    /// <remarks>
    ///     This property specifies the table into which rows will be inserted during a bulk operation.
    ///     It must be a valid PostgreSQL table name and is required for the execution of the corresponding database action.
    /// </remarks>
    /// <value>
    ///     A string containing the name of the table where the data will be inserted.
    /// </value>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
    ///     Thrown when the property is not specified or is null.
    /// </exception>
    [Required]
    public required string Table { get; init; }

    /// <summary>
    ///     Represents the collection of column names in the target PostgreSQL table for batch insertion.
    /// </summary>
    /// <remarks>
    ///     This property specifies the names of the columns into which the data will be inserted during a bulk operation.
    ///     The column names must correspond to those in the target table in the same order as the data values are provided.
    /// </remarks>
    /// <value>
    ///     An array of strings containing the names of the columns that will be used for data insertion.
    /// </value>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
    ///     Thrown when the property is not specified, is null, or contains invalid column names.
    /// </exception>
    [Required]
    public required string[] Columns { get; init; }

    /// <summary>
    ///     Represents the matrix of data values to be inserted into the PostgreSQL table.
    /// </summary>
    /// <remarks>
    ///     This property contains a two-dimensional array where each inner array represents a row of data,
    ///     and each element within the inner array corresponds to a column value for that row.
    ///     The data is used during a batch insertion operation using the PostgreSQL COPY command.
    /// </remarks>
    /// <value>
    ///     A jagged array where each nested array denotes a row, and each element within the nested array
    ///     represents a value for the corresponding column in the provided table.
    /// </value>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
    ///     Thrown when the property is not provided, is null, or does not align with the structure of the specified columns.
    /// </exception>
    [Required]
    public required object[][] Values { get; init; }
}