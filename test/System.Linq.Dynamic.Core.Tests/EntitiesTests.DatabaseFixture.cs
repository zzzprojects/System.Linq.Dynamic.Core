using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

/// <summary>
/// https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
/// </summary>
public class EntitiesTestsDatabaseFixture : IAsyncLifetime
{
    // https://github.com/microsoft/mssql-docker/issues/892
    private readonly Lazy<MsSqlContainer> _msSqlContainer = new(() => new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build());

    public string ConnectionString => _msSqlContainer.Value.GetConnectionString();

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

        await _msSqlContainer.Value.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (UseInMemory)
        {
            return;
        }

        await _msSqlContainer.Value.DisposeAsync();
    }
}