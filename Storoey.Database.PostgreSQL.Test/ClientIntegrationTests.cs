using Microsoft.Extensions.Configuration;
using Storoey.Database.PostgreSQL.Options;
using Storoey.Database.PostgreSQL.Parameters;
using Xunit;
using Xunit.Abstractions;

namespace Storoey.Database.PostgreSQL.Test;

public class ClientIntegrationTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Client _client;

    public ClientIntegrationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var configuration = new ConfigurationBuilder().AddUserSecrets<ClientIntegrationTests>().Build();
        _client = new Client(new ClientOptions
        {
            Host = configuration["Host"] ?? "localhost",
            Port = int.Parse(configuration["Port"] ?? "5432"),
            Database = configuration["Database"] ?? "postgres",
            Username = configuration["Username"] ?? "postgres",
            Password = configuration["Password"] ?? "<PASSWORD>",
            MachineId = 1
        });
    }

    [Fact]
    public async Task Test()
    {
        await _client.Insert(new InsertParameter
        {
            CommandText = "INSERT INTO bulkTest (id, name, title) VALUES ($1, $2, $3)",
            Values = [_client.NextIdentity(), "Johnny", "User"],
        });
    }
    
    [Fact]
    public async Task Test1()
    {
        await _client.InsertBatch(new InsertBatchParameter
        {
            Table = "bulkTest",
            Columns = ["id", "name", "title"],
            Values = [
                [Guid.NewGuid(), "Fredrik", "Owner"],
                [Guid.NewGuid(), "Pablo", DBNull.Value]
            ],
        });
    }

    [Fact]
    public async Task BulkLarge()
    {
        var start = DateTime.Now;
        
        await _client.InsertBatch(new InsertBatchParameter
        {
            Table = "bulkTest2",
            Columns = ["id", "name", "title"],
            Values = Enumerable.Range(0, 10_000_00).Select(i => new object[] { _client.NextIdentity(), $"Name {i}", $"Title {i}" }).ToArray(),
        });
        
        var end = DateTime.Now;
        
        _testOutputHelper.WriteLine((end - start).ToString("c"));
    }

    [Fact]
    public async Task FirstOrDefault()
    {
        var result = await _client.FirstOrDefault(new WhereParameter
        {
            CommandText = "SELECT * FROM bulkTest2 WHERE id = $1",
            Parameters = [7275673987122729055]
        });
        
        Assert.NotNull(result);
        Assert.Equal("Name 95", result["name"]);
    }
    
    [Fact]
    public async Task Where()
    {
        var result = await _client.Where(new WhereParameter
        {
            CommandText = "SELECT * FROM bulkTest2 WHERE id < $1 OR id > $2 ORDER BY id",
            Parameters = [7275673987122728970, 7275673996270509902]
        });
        
        Assert.NotEmpty(result);
        Assert.Equal(20, result.Length);
        
        Assert.Equal("Name 0", result[0]["name"]);
        Assert.Equal("Name 1", result[1]["name"]);
        Assert.Equal("Name 2", result[2]["name"]);
        Assert.Equal("Name 3", result[3]["name"]);
        Assert.Equal("Name 4", result[4]["name"]);
        Assert.Equal("Name 5", result[5]["name"]);
        Assert.Equal("Name 6", result[6]["name"]);
        Assert.Equal("Name 7", result[7]["name"]);
        Assert.Equal("Name 8", result[8]["name"]);
        Assert.Equal("Name 9", result[9]["name"]);
        
        Assert.Equal("Name 999990", result[10]["name"]);
        Assert.Equal("Name 999991", result[11]["name"]);
        Assert.Equal("Name 999992", result[12]["name"]);
        Assert.Equal("Name 999993", result[13]["name"]);
        Assert.Equal("Name 999994", result[14]["name"]);
        Assert.Equal("Name 999995", result[15]["name"]);
        Assert.Equal("Name 999996", result[16]["name"]);
        Assert.Equal("Name 999997", result[17]["name"]);
        Assert.Equal("Name 999998", result[18]["name"]);
        Assert.Equal("Name 999999", result[19]["name"]);

    }
}