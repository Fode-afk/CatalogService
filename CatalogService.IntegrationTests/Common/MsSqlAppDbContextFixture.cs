using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace CatalogService.IntegrationTests.Common;

internal sealed class MsSqlAppDbContextFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest").Build();

    public AppDbContext Context { get; private set; } = null!;
    public RecordingDomainEventsDispatcher Dispatcher { get; private set; } = null!;

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        Dispatcher = new RecordingDomainEventsDispatcher();
        Context = new AppDbContext(options, Dispatcher);

        await Context.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _container.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
