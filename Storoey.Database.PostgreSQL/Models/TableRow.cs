using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace Storoey.Database.PostgreSQL.Models;

/// <summary>
/// Represents a single row in a table, consisting of multiple columns.
/// </summary>
public sealed record TableRow
{
    /// <summary>
    /// Represents the collection of columns in a single table row.
    /// </summary>
    /// <remarks>
    /// Each column is represented by a <see cref="TableColumn"/> object, containing
    /// metadata such as the column name, data type, and value. This property is required
    /// and must be initialized with an array of <see cref="TableColumn"/> objects.
    /// </remarks>
    [Required]
    public required TableColumn[] Columns { get; init; }

    /// <summary>
    /// Retrieves the value of a specific column within the row by its column name.
    /// </summary>
    /// <param name="columnName">The name of the column whose value is to be retrieved.</param>
    /// <returns>The value of the specified column, or null if the column is not found.</returns>
    public object? this[string columnName] => Columns.Single(x => x.Name == columnName).Value;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnName"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public TValue? Column<TValue>(string columnName)
    {
        var value = Columns.Single(x => x.Name == columnName).Value;

        return value switch
        {
            null or DBNull => default,
            _ => (TValue)value
        };
    }
}