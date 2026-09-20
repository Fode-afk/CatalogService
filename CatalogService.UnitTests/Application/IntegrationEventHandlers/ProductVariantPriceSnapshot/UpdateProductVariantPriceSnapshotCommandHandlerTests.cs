using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.UpdateProductVariantPriceSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.Models;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using migApp.Shared.Enums.Products;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.ProductVariantPriceSnapshot;

public class UpdateProductVariantPriceSnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();
    private readonly ILogger<UpdateProductVariantPriceSnapshotCommandHandler> _logger =
        Substitute.For<ILogger<UpdateProductVariantPriceSnapshotCommandHandler>>();

    [Fact]
    public async Task Should_Throw_And_Record_Metric_When_Snapshot_Not_Found()
    {
        // Arrange
        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);

        var handler = new UpdateProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantPriceSnapshotCommand();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        _metrics.Received(1).RecordSnapshotNotFound("ProductVariant");
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Skip_Update_And_Record_Metric_When_Version_Is_Outdated()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productVariantId: productVariantId, version: 5);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);

        var handler = new UpdateProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantPriceSnapshotCommand(
            productVariantId, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateProductVariantPriceSnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_And_Record_Metric_When_Product_Not_Found()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var orphanProductId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: orphanProductId, productVariantId: productVariantId, version: 1);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { existingSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new UpdateProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantPriceSnapshotCommand(
            productVariantId, version: 2);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
        _metrics.Received(1).RecordProductNotFound();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Suspend_Product_When_Price_Removed_And_No_Other_Variant_Has_Price()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished();
        var existingSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: productVariantId, hasPrice: true, version: 1);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { existingSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new UpdateProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantPriceSnapshotCommand(
            productVariantId, hasPrice: false, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        existingSnapshot.HasPrice.Should().BeFalse();
        existingSnapshot.Version.Should().Be(2);
        _metrics.Received(1).RecordProductSuspended(Arg.Any<string>());
        _metrics.DidNotReceive().RecordProductRestored();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Suspend_Product_When_Price_Removed_But_Other_Variant_Has_Price()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var otherVariantId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished();

        var existingSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: productVariantId, hasPrice: true, version: 1);
        var otherSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: otherVariantId, hasPrice: true);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>
        {
            existingSnapshot, otherSnapshot
        }.BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new UpdateProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantPriceSnapshotCommand(
            productVariantId, hasPrice: false, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.DidNotReceive().RecordProductSuspended(Arg.Any<string>());
        _metrics.DidNotReceive().RecordProductRestored();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Fail_When_TryRestore_Is_Called_From_Suspended_Status()
    {
        var productVariantId = Guid.NewGuid();
        var product = ProductTestFactory.CreateSuspended(null, null, null,
            new ProductSuspensionReason(SuspensionReason.NoPriceAvailable));

        var existingSnapshot = SnapshotTestsFactory.PriceSnapshot(
            productId: product.Id, productVariantId: productVariantId, hasPrice: false, version: 1);

        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot> { existingSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        var handler = new UpdateProductVariantPriceSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantPriceSnapshotCommand(
            productVariantId, hasPrice: true, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordProductRestored();
    }
}
