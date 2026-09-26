using CatalogService.Application.Interfaces.Data;
using CatalogService.Infrastructure.BackgroundServices;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace CatalogService.IntegrationTests.Infrastructure.BackgroundServices;

public class SoftDeletedProductsCleanupServiceTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() =>
        _fixture.InitializeAsync();

    public ValueTask DisposeAsync() =>
        _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Delete_SoftDeleted_Products_Older_Than_Retention_Period()
    {
        // Arrange
        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var oldDeletedAt = now.AddDays(-31);

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(suffix: i.ToString()))
            .ToList();

        foreach (var product in products)
        {
            product.Delete(
                ProductContextsTestFactory.ValidDeleteContext(),
                oldDeletedAt);
        }

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var service = CreateService(
            now,
            retentionPeriod: TimeSpan.FromDays(30));

        // Act
        await service.CleanupAsync(_cancellationToken);

        // Assert
        var remaining = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => products.Select(x => x.Id).Contains(p.Id))
            .ToListAsync(_cancellationToken);

        remaining.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Not_Delete_SoftDeleted_Products_Within_Retention_Period()
    {
        // Arrange
        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var recentlyDeletedAt = now.AddDays(-29);

        var product = ProductTestFactory.CreateValid();

        product.Delete(
            ProductContextsTestFactory.ValidDeleteContext(),
            recentlyDeletedAt);

        _fixture.Context.Products.Add(product);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var service = CreateService(
            now,
            retentionPeriod: TimeSpan.FromDays(30));

        // Act
        await service.CleanupAsync(_cancellationToken);

        // Assert
        var remaining = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                p => p.Id == product.Id,
                _cancellationToken);

        remaining.Should().NotBeNull();
        remaining!.DeletedAt.Should().Be(recentlyDeletedAt);
    }

    [Fact]
    public async Task Should_Not_Delete_Active_Products()
    {
        // Arrange
        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var product = ProductTestFactory.CreateValid();

        _fixture.Context.Products.Add(product);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var service = CreateService(
            now,
            retentionPeriod: TimeSpan.FromDays(30));

        // Act
        await service.CleanupAsync(_cancellationToken);

        // Assert
        var remaining = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                p => p.Id == product.Id,
                _cancellationToken);

        remaining.Should().NotBeNull();
        remaining!.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task Should_Process_Products_In_Batches()
    {
        // Arrange
        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var oldDeletedAt = now.AddDays(-31);

        var products = Enumerable.Range(0, 1200)
            .Select(i =>
            {
                var product = ProductTestFactory.CreateValid(suffix: i.ToString());

                product.Delete(
                    ProductContextsTestFactory.ValidDeleteContext(),
                    oldDeletedAt);

                return product;
            })
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var service = CreateService(
            now,
            retentionPeriod: TimeSpan.FromDays(30),
            batchSize: 500);

        // Act
        await service.CleanupAsync(_cancellationToken);

        // Assert
        var remaining = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => products.Select(x => x.Id).Contains(p.Id))
            .CountAsync(_cancellationToken);

        remaining.Should().Be(0);
    }

    [Fact]
    public async Task Should_Delete_Product_Exactly_At_Retention_Cutoff()
    {
        // Arrange
        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var deletedAt = now.AddDays(-30);

        var product = ProductTestFactory.CreateValid();

        product.Delete(
            ProductContextsTestFactory.ValidDeleteContext(),
            deletedAt);

        _fixture.Context.Products.Add(product);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var service = CreateService(
            now,
            retentionPeriod: TimeSpan.FromDays(30));

        // Act
        await service.CleanupAsync(_cancellationToken);

        // Assert
        var remaining = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                p => p.Id == product.Id,
                _cancellationToken);

        remaining.Should().BeNull();
    }

    private SoftDeletedProductsCleanupService CreateService(
        DateTimeOffset now,
        TimeSpan? retentionPeriod = null,
        int batchSize = 500)
    {
        var services = new ServiceCollection();

        services.AddScoped<IAppDbContext>(_ => _fixture.Context);

        var serviceProvider = services.BuildServiceProvider();

        var scopeFactory =
            serviceProvider.GetRequiredService<IServiceScopeFactory>();

        var options = new SoftDeletedProductsCleanupOptions
        {
            Interval = TimeSpan.FromHours(6),
            RetentionPeriod =
                retentionPeriod ?? TimeSpan.FromDays(30),
            BatchSize = batchSize
        };

        return new SoftDeletedProductsCleanupService(
            scopeFactory,
            new FakeTimeProvider(now),
            options,
            NullLogger<SoftDeletedProductsCleanupService>.Instance);
    }
}
