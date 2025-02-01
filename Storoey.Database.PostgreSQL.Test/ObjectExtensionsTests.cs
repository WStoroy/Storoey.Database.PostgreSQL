using System.Dynamic;
using NpgsqlTypes;
using Storoey.Database.PostgreSQL.Exceptions;
using Storoey.Database.PostgreSQL.Extensions;
using Xunit;

namespace Storoey.Database.PostgreSQL.Test;

public class ObjectExtensionsTests
{
    [Theory]
    [MemberData(nameof(GetTypeCorrectTestData))]
    public void Extension_ToNpgsqlDbType_ReturnsCorrectTypeFromValue(object inputValue, NpgsqlDbType expectedType)
    {
        var result = inputValue.ToNpgsqlDbType();

        Assert.Equal(expectedType, result);
    }

    [Theory]
    [MemberData(nameof(GetTypeCorrectTestData))]
    public void Extension_TryToNpgsqlDbType_ReturnsCorrectTypeFromValue(object inputValue, NpgsqlDbType expectedType)
    {
        var result = inputValue.TryToNpgsqlDbType(out var actualType);

        Assert.True(result);
        Assert.Equal(expectedType, actualType);
    }

    [Theory]
    [MemberData(nameof(GetTypeCorrectTestData))]
    public void Extension_ToNpgsqlParameter_ReturnsCorrectParameter(object inputValue,
        NpgsqlDbType expectedType)
    {
        var result = inputValue.ToNpgsqlParameter();

        Assert.NotNull(result);
        Assert.Equal(inputValue, result.Value);
        Assert.Equal(expectedType, result.NpgsqlDbType);
    }

    [Fact]
    public void Extension_ToNpgsqlDbType_ThrowsExceptionForUnknownType()
    {
        var unknownType = new ExpandoObject();
        Assert.Throws<UnknownTypeException>(() => unknownType.ToNpgsqlDbType());
    }

    [Fact]
    public void Extension_TryToNpgsqlDbType_ReturnsFalseForUnknownType()
    {
        var unknownType = new ExpandoObject();
        var result = unknownType.TryToNpgsqlDbType(out var actualType);

        Assert.False(result);
        Assert.Equal(NpgsqlDbType.Unknown, actualType);
    }

    [Fact]
    public void Extension_ToNpgsqlParameter_ThrowsExceptionForUnknownType()
    {
        var unknownType = new ExpandoObject();
        Assert.Throws<UnknownTypeException>(() => unknownType.ToNpgsqlParameter());
    }

    public static IEnumerable<object[]> GetTypeCorrectTestData()
    {
        return new List<object[]>
        {
            new object[] { Guid.NewGuid(), NpgsqlDbType.Uuid },
            new object[] { "this_is_a_string", NpgsqlDbType.Varchar },
            new object[] { 12, NpgsqlDbType.Integer },
            new object[] { false, NpgsqlDbType.Boolean },
            new object[] { true, NpgsqlDbType.Boolean },
            new object[] { DateTimeOffset.Now, NpgsqlDbType.TimestampTz },
            new object[] { 47.13d, NpgsqlDbType.Double },
            new object[] { 0.89m, NpgsqlDbType.Numeric },
            new object[] { new byte[] { 2, 3, 4, 5 }, NpgsqlDbType.Bytea },
            new object[] { TimeSpan.FromSeconds(23), NpgsqlDbType.Interval },
            new object[] { DateTime.Now, NpgsqlDbType.Timestamp },
            new object[] { (byte)255, NpgsqlDbType.Smallint },
            new object[] { (short)256, NpgsqlDbType.Smallint },
            new object[] { 72.3f, NpgsqlDbType.Real },
            new object[] { 'h', NpgsqlDbType.Char },
            new object[] { new[] { 'h', 'e', 'l', 'l', 'o' }, NpgsqlDbType.Varchar },
            new object[] { 862817670527975424L, NpgsqlDbType.Bigint }
        };
    }
}