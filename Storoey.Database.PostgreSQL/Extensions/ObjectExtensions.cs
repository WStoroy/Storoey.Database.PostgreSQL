using Npgsql;
using NpgsqlTypes;
using Storoey.Database.PostgreSQL.Exceptions;

namespace Storoey.Database.PostgreSQL.Extensions;

/// <summary>
///     Provides extension methods for converting .NET objects to PostgreSQL-compatible types using Npgsql.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///     A readonly dictionary that maps .NET types to their corresponding NpgsqlDbType values.
    ///     This dictionary is used for type conversion between .NET types and PostgreSQL types
    ///     when interacting with PostgreSQL databases using the Npgsql library.
    /// </summary>
    private static readonly Dictionary<Type, NpgsqlDbType> DotNetTypeToNpgsqlDbType = new()
    {
        { typeof(Guid), NpgsqlDbType.Uuid },
        { typeof(string), NpgsqlDbType.Varchar },
        { typeof(int), NpgsqlDbType.Integer },
        { typeof(bool), NpgsqlDbType.Boolean },
        { typeof(DateTimeOffset), NpgsqlDbType.TimestampTz },
        { typeof(double), NpgsqlDbType.Double },
        { typeof(decimal), NpgsqlDbType.Numeric },
        { typeof(byte[]), NpgsqlDbType.Bytea },
        { typeof(TimeSpan), NpgsqlDbType.Interval },
        { typeof(DateTime), NpgsqlDbType.Timestamp },
        { typeof(byte), NpgsqlDbType.Smallint },
        { typeof(short), NpgsqlDbType.Smallint },
        { typeof(float), NpgsqlDbType.Real },
        { typeof(char), NpgsqlDbType.Char },
        { typeof(char[]), NpgsqlDbType.Varchar },
        { typeof(long), NpgsqlDbType.Bigint }
    };

    /// <summary>
    ///     Converts the given .NET object to its corresponding NpgsqlDbType.
    /// </summary>
    /// <param name="value">The object whose type is to be converted to an NpgsqlDbType.</param>
    /// <returns>The NpgsqlDbType corresponding to the .NET type of the provided object.</returns>
    /// <exception cref="Exception">
    ///     Thrown when the .NET type of the provided object does not have a corresponding
    ///     NpgsqlDbType.
    /// </exception>
    public static NpgsqlDbType ToNpgsqlDbType(this object value)
    {
        var dotnetType = value.GetType();

        if (!DotNetTypeToNpgsqlDbType.TryGetValue(dotnetType, out var npgsqlDbType))
        {
            throw new UnknownTypeException($"Unknown .NET type: {dotnetType.FullName}");
        }

        return npgsqlDbType;
    }

    /// <summary>
    ///     Attempts to convert the specified .NET object to its corresponding NpgsqlDbType.
    /// </summary>
    /// <param name="value">The object whose type is to be converted to an NpgsqlDbType.</param>
    /// <param name="npgsqlDbType">
    ///     When this method returns, contains the NpgsqlDbType corresponding to the .NET type of the object,
    ///     if the conversion succeeded. Otherwise, the value is undefined.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the .NET type of the provided object has a corresponding NpgsqlDbType and the conversion succeeded;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public static bool TryToNpgsqlDbType(this object value, out NpgsqlDbType npgsqlDbType)
    {
        var dotnetType = value.GetType();

        if (DotNetTypeToNpgsqlDbType.TryGetValue(dotnetType, out npgsqlDbType))
        {
            return true;
        }

        Console.WriteLine($"Unknown .NET type: {dotnetType.FullName}");
        npgsqlDbType = NpgsqlDbType.Unknown;
        return false;
    }

    /// <summary>
    ///     Converts the given .NET object to an NpgsqlParameter for use with Npgsql commands.
    /// </summary>
    /// <param name="value">The .NET object to be converted into an NpgsqlParameter.</param>
    /// <returns>An NpgsqlParameter configured with the provided object's value and its corresponding NpgsqlDbType.</returns>
    /// <exception cref="Exception">
    ///     Thrown when the .NET type of the provided object does not have a corresponding
    ///     NpgsqlDbType.
    /// </exception>
    public static NpgsqlParameter ToNpgsqlParameter(this object value)
    {
        var dotnetType = value.GetType();

        if (!DotNetTypeToNpgsqlDbType.TryGetValue(dotnetType, out var npgsqlDbType))
        {
            throw new UnknownTypeException($"Unknown .NET type: {dotnetType.FullName}");
        }

        return new NpgsqlParameter
        {
            NpgsqlValue = value,
            NpgsqlDbType = npgsqlDbType
        };
    }
}