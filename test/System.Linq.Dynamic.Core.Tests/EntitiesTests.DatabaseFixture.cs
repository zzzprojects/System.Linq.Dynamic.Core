using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

/// <summary>
/// https://blog.jetbrains.com/dotnet/2023/10/24/how-to-use-testcontainers-with-dotnet-unit-tests/
/// </summary>
public class EntitiesTestsDatabaseFixture : IAsyncLifetime
{
    private readonly Testcontainers.MsSql.MsSqlContainer _msSqlContainer = new Testcontainers.MsSql.MsSqlBuilder().Build();

    public string ConnectionString => _msSqlContainer.GetConnectionString();

    public string ContainerId => $"{_msSqlContainer.Id}";

    public Task InitializeAsync() => _msSqlContainer.StartAsync();

    public Task DisposeAsync() => _msSqlContainer.DisposeAsync().AsTask();
}