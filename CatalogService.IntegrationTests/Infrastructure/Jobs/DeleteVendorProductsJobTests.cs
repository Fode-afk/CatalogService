using CatalogService.Infrastructure.Jobs;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace CatalogService.IntegrationTests.Infrastructure.Jobs;

public class DeleteVendorProductsJobTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Delete_All_Active_Products_Of_Vendor()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(vendorId, suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var timeProvider = new FakeTimeProvider(now);

        var job = new DeleteVendorProductsJob(
            _fixture.Context,
            timeProvider);

        // Act
        await job.Execute(vendorId, cancellationToken);

        // Assert
        var deletedProducts = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.VendorId == vendorId)
            .ToListAsync(cancellationToken);

        deletedProducts.Should().HaveCount(3);
        deletedProducts.Should().OnlyContain(p => p.DeletedAt == now);
    }

    [Fact]
    public async Task Should_Not_Delete_Already_Deleted_Products()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var existingDeletedAt = new DateTimeOffset(2026, 9, 22, 12, 0, 0, TimeSpan.Zero);

        var activeProduct = ProductTestFactory.CreateValid(vendorId, suffix: "1");

        var deletedProduct = ProductTestFactory.CreateValid(vendorId, suffix: "2");
        deletedProduct.Delete(
            ProductContextsTestFactory.ValidDeleteContext(),
            existingDeletedAt);

        _fixture.Context.Products.AddRange(
            activeProduct,
            deletedProduct);

        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new DeleteVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(vendorId, cancellationToken);

        // Assert
        var products = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.VendorId == vendorId)
            .ToListAsync(cancellationToken);

        products.Should().HaveCount(2);

        products.Single(p => p.Id == activeProduct.Id)
            .DeletedAt.Should().Be(now);

        products.Single(p => p.Id == deletedProduct.Id)
            .DeletedAt.Should().Be(existingDeletedAt);
    }

    [Fact]
    public async Task Should_Not_Delete_Products_Belonging_To_Another_Vendor()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var anotherVendorId = Guid.NewGuid();

        var vendorProduct = ProductTestFactory.CreateValid(vendorId, suffix: "1");
        var anotherVendorProduct = ProductTestFactory.CreateValid(anotherVendorId, suffix: "2");

        _fixture.Context.Products.AddRange(
            vendorProduct,
            anotherVendorProduct);

        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new DeleteVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(vendorId, cancellationToken);

        // Assert
        var products = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p =>
                p.Id == vendorProduct.Id ||
                p.Id == anotherVendorProduct.Id)
            .ToListAsync(cancellationToken);

        products
            .Single(p => p.Id == vendorProduct.Id)
            .DeletedAt
            .Should()
            .Be(now);

        products
            .Single(p => p.Id == anotherVendorProduct.Id)
            .DeletedAt
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task Should_Process_Products_In_Batches()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var products = Enumerable.Range(0, 250)
            .Select(i => ProductTestFactory.CreateValid(vendorId, suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new DeleteVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(vendorId, cancellationToken);

        // Assert
        var remainingActiveProducts =
            await _fixture.Context.Products
                .Where(p =>
                    p.VendorId == vendorId &&
                    p.DeletedAt == null)
                .IgnoreQueryFilters()
                .CountAsync(cancellationToken);

        remainingActiveProducts.Should().Be(0);
    }

    [Fact]
    public async Task Should_Respect_Cancellation()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(vendorId, suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new DeleteVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = () => job.Execute(vendorId, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}