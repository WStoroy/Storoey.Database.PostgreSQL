using System.ComponentModel.DataAnnotations;

namespace Storoey.Database.PostgreSQL.Parameters;

/// <summary>
///     Represents a parameterized WHERE clause for constructing and executing SQL queries.
/// </summary>
/// <remarks>
///     This record is used to encapsulate the SQL command text and corresponding parameters required to execute the query.
///     The <see cref="CommandText" /> property contains the SQL query or statement, while the optional
///     <see cref="Parameters" />
///     property holds the values to be substituted into the query's parameter placeholders.
/// </remarks>
public sealed record WhereParameter
{
    /// <summary>
    ///     Gets the SQL query or statement that represents the parameterized WHERE clause.
    /// </summary>
    /// <remarks>
    ///     The <c>CommandText</c> property contains the raw SQL command used to construct and execute database queries.
    ///     It should include the SQL syntax required to specify the desired operation, such as SELECT, UPDATE, or DELETE,
    ///     and the accompanying WHERE clause.
    ///     This property is required and must be initialized with a valid SQL query string.
    /// </remarks>
    [Required]
    public required string CommandText { get; init; }

    /// <summary>
    ///     Gets or sets the values for the parameterized placeholders in the SQL query.
    /// </summary>
    /// <remarks>
    ///     The <c>Parameters</c> property contains an array of objects that represent the values to be injected
    ///     into the SQL query's parameterized placeholders at runtime.
    ///     When creating parameterized SQL commands, this property should align with the placeholders defined in the
    ///     <c>CommandText</c>. If no parameters are required, this property can be <c>null</c>.
    /// </remarks>
    public object[]? Parameters { get; init; }
}