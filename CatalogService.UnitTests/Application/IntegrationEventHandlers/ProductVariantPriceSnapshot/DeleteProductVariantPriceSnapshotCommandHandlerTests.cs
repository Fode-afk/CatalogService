using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Models;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.ProductVariantPriceSnapshot;

public class DeleteProductVariantPriceSnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();
    private readonly ILogger<DeleteProductVariantPriceSnapshotCommandHandler> _logger =
        Substitute.For<ILogger<DeleteProductVariantPriceSnapshotCommandHandler>>();

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Not_Found()
    {
        // Arrange
        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);

        var handler = new DeleteProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantPriceSnapshotCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        priceSet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>());
    }

    [Fact]
    public async Task Should_Remove_Snapshot_Without_Suspending_When_Product_Not_Found()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var orphanProductId = Guid.NewGuid();
        var priceSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: orphanProductId, productVariantId: productVariantId);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { priceSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new DeleteProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantPriceSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        priceSet.Received(1).Remove(priceSnapshot);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductSuspended(Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Remove_Snapshot_Without_Suspending_When_Product_Is_Already_Deleted()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var deletedProduct = ProductTestFactory.CreateDeleted();
        var priceSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: deletedProduct.Id, productVariantId: productVariantId);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { priceSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { deletedProduct }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new DeleteProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantPriceSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        priceSet.Received(1).Remove(priceSnapshot);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductSuspended(Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Remove_Snapshot_Without_Suspending_When_Other_Variant_Has_Price()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var otherVariantId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished();

        var priceSnapshotToDelete = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: productVariantId, hasPrice: true);
        var otherPriceSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: otherVariantId, hasPrice: true);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>
        {
            priceSnapshotToDelete, otherPriceSnapshot
        }.BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new DeleteProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantPriceSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        priceSet.Received(1).Remove(priceSnapshotToDelete);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductSuspended(Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Suspend_Product_And_Record_Metric_When_No_Other_Variant_Has_Price()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished();

        var priceSnapshotToDelete = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: productVariantId, hasPrice: true);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { priceSnapshotToDelete }
            .BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new DeleteProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantPriceSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        priceSet.Received(1).Remove(priceSnapshotToDelete);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.Received(1).RecordProductSuspended(Arg.Any<string>());
    }
}