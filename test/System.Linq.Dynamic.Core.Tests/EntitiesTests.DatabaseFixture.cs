using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

/// <summary>
/// https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
/// </summary>
public class EntitiesTestsDatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public string ConnectionString => _msSqlContainer.GetConnectionString();

    public string ContainerId => $"{_msSqlContainer.Id}";

    public bool UseInMemory
    {
        get
        {
            var useInMemory = Environment.GetEnvironmentVariable("UseInMemory");
            return bool.TryParse(useInMemory, out var value) && value;
        }
    }

    public async Task InitializeAsync()
    {
        if (UseInMemory)
        {
            return;
        }

        await _msSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (UseInMemory)
        {
            return;
        }

        await _msSqlContainer.DisposeAsync();
    }
}