using System.ComponentModel.DataAnnotations;

namespace Storoey.Database.PostgreSQL.Models;

/// <summary>
///     Represents a column in a database table row.
/// </summary>
/// <remarks>
///     This class is used to describe individual columns in a table row,
///     containing information such as the column name, data type, and the value it holds.
/// </remarks>
public sealed record TableColumn
{
    /// <summary>
    ///     Gets the name of the column in a database table.
    /// </summary>
    /// <remarks>
    ///     Represents the identifier or header of a specific column in the table structure.
    ///     This property is essential for referencing the column within a row or when performing database operations.
    /// </remarks>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the value stored in the column.
    /// </summary>
    /// <remarks>
    ///     Represents the data associated with the column for a specific row in a database table.
    ///     The value can be of any type depending on the column's data type definition.
    ///     This property is essential for retrieving or processing the column's data.
    /// </remarks>
    [Required]
    public object? Value { get; init; }

    /// <summary>
    ///     Gets the data type of the column in a database table.
    /// </summary>
    /// <remarks>
    ///     Represents the .NET type of the data stored in the column. This property is used
    ///     to reflect the underlying type of the column value (e.g., string, int, DateTime).
    ///     It is helpful for type conversion and schema interpretation during runtime operations.
    /// </remarks>
    [Required]
    public required Type Type { get; init; }
}