using Npgsql;

namespace Storoey.Database.PostgreSQL.Parameters;

public record MappedWhereParameter<T> : WhereParameter
{
    public required Func<NpgsqlDataReader, T> Map { get; init; }
}