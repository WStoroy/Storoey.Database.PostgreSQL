using IdGen;

namespace Storoey.Database.PostgreSQL.Extensions;

/// <summary>
/// Provides extension methods for the SnowflakeIDGenerator class.
/// </summary>
public static class SnowflakeIdGeneratorExtensions
{
    /// <summary>
    /// Generates the next unique Snowflake ID using the provided generator.
    /// </summary>
    /// <param name="generator">An instance of the SnowflakeIDGenerator used to generate the unique ID.</param>
    /// <returns>A long integer representing the unique Snowflake ID.</returns>
    public static long Next(this IdGenerator generator)
    {
        return generator.CreateId();
    }
}