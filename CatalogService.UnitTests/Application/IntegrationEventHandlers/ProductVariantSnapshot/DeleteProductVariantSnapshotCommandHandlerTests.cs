using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.Models;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.ProductVariantSnapshot;

public class DeleteProductVariantSnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();
    private readonly ILogger<DeleteProductVariantSnapshotCommandHandler> _logger =
        Substitute.For<ILogger<DeleteProductVariantSnapshotCommandHandler>>();

    public DeleteProductVariantSnapshotCommandHandlerTests()
    {
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Not_Found()
    {
        // Arrange
        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);

        var handler = new DeleteProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantSnapshotCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        variantSet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantSnapshot>());
    }

    [Fact]
    public async Task Should_Remove_Snapshot_Without_Touching_Domain_When_Product_Not_Found()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var orphanProductId = Guid.NewGuid();
        var variantSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productId: orphanProductId, productVariantId: productVariantId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { variantSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);

        var handler = new DeleteProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        variantSet.Received(1).Remove(variantSnapshot);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordSnapshotNotFound(Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Remove_Snapshot_Without_Touching_Domain_When_Product_Is_Already_Deleted()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var deletedProduct = ProductTestFactory.CreateDeleted();
        var variantSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productId: deletedProduct.Id, productVariantId: productVariantId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { variantSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { deletedProduct }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);

        var handler = new DeleteProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        variantSet.Received(1).Remove(variantSnapshot);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_Without_Removing_Snapshot_When_VendorSnapshot_Not_Found()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);
        var variantSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productId: product.Id, productVariantId: productVariantId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { variantSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new DeleteProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantSnapshotCommand(productVariantId);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        _metrics.Received(1).RecordSnapshotNotFound("Vendor");
        variantSet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantSnapshot>());
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Remove_Snapshot_And_Remove_Variant_From_Attributes_When_Successful()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId, categoryId: categoryId);
        var variantId = Guid.NewGuid();

        product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [ProductDataTestFactory.CreateVariantValue(characteristicId: characteristicId, valueId: variantId)],
            TestClock.DefaultNow);

        var variantSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productId: product.Id, productVariantId: variantId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { variantSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new DeleteProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantSnapshotCommand(variantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        variantSet.Received(1).Remove(variantSnapshot);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Log_Warning_And_Still_Remove_Snapshot_When_RemoveVariantFromAttributes_Fails()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        var product = ProductTestFactory.CreateValid();
        product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [ProductDataTestFactory.CreateVariantValue(characteristicId: characteristicId, valueId: variantId)],
            TestClock.DefaultNow);

        product.Archive(ProductContextsTestFactory.ValidArchiveContext(), TestClock.DefaultNow);

        var variantSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productId: product.Id, productVariantId: variantId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { variantSnapshot }
            .BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(product.VendorId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new DeleteProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidDeleteProductVariantSnapshotCommand(variantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        variantSet.Received(1).Remove(variantSnapshot);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _logger.VerifyLog(LogLevel.Warning, new EventId(1005));
    }
}