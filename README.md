
# Storoey.Database.PostgreSQL

## Description
This project is a PostgreSQL client library for .NET 8.0, using Npgsql for database interaction and SnowflakeIDGenerator for unique ID creation.

## Features
- Comprehensive PostgreSQL client support for .NET 8.0.
- Utilizes the `Npgsql` library for database connectivity and operations.
- Supports unique identifier generation with `SnowflakeIDGenerator`.
- Designed for scalability and high-performance database interactions.

## Usage
To use the library, configure it in your .NET project as follows:

### Creating a client
```csharp
using Storoey.Database.PostgreSQL;

new Client(new ClientOptions
{
    Host = "localhost",
    Port = 5432,
    Database = "postgres",
    Username = "postgres",
    Password = "<PASSWORD>",
    MachineId = 1
});
```

### Insert a row
```csharp
await _client.Insert(new InsertParameter
{
    CommandText = "INSERT INTO table (id, name, title) VALUES ($1, $2, $3)",
    Values = [_client.NextIdentity(), "Johnny", "User"],
});
```

### Bulk insert
```csharp
await _client.InsertBatch(new InsertBatchParameter
{
    Table = "tableName",
    Columns = ["id", "name", "title"],
    Values = Enumerable.Range(0, 10_000_00).Select(i => new object[] { _client.NextIdentity(), $"Name {i}", $"Title {i}" }).ToArray(),
});
```

### Select single row
```csharp
TableRow? tableRow = await _client.FirstOrDefault(new WhereParameter
{
    CommandText = "SELECT * FROM tableName WHERE id = $1",
    Parameters = [7275673987122729055]
});
```
_**Note:** If no limit is provided, a default limit of 1 will be applied automatically._

### Select multiple rows
```csharp
TableRow[] tableRows = await _client.Where(new WhereParameter
{
    CommandText = "SELECT * FROM tableName WHERE id < $1 OR id > $2 ORDER BY id",
    Parameters = [7275673987122728970, 7275673996270509902]
});
```

---

_**Disclaimer:** All comments in this code were lovingly crafted by ChatGPT.  
Any resemblance to intelligent human commentary is purely coincidental.  
Bugs are still 100% my fault though._