using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;
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

public class AddProductVariantSnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();
    private readonly ILogger<AddProductVariantSnapshotCommandHandler> _logger =
        Substitute.For<ILogger<AddProductVariantSnapshotCommandHandler>>();

    public AddProductVariantSnapshotCommandHandlerTests()
    {
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Already_Exists()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>
        {
            new() { ProductVariantId = productVariantId, ProductId = Guid.NewGuid() }
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        variantSet.DidNotReceive().Add(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantSnapshot>());
    }

    [Fact]
    public async Task Should_Throw_And_Record_Metric_When_Product_Not_Found()
    {
        // Arrange
        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
        _metrics.Received(1).RecordProductNotFound();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_And_Record_Metric_When_VendorSnapshot_Not_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(productId: product.Id);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        _metrics.Received(1).RecordSnapshotNotFound("Vendor");
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_When_Characteristic_Snapshot_Is_Missing()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId, categoryId: categoryId);
        var missingCharacteristicId = Guid.NewGuid();

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(
            productId: product.Id,
            characteristicValues: [SnapshotCommandTestsFactory.ValidVariantAttributeDto(missingCharacteristicId)]);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Add_Snapshot_When_No_Characteristic_Values_Provided()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(
            productId: product.Id, characteristicValues: []);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        variantSet.Received(1).Add(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantSnapshot>());
    }

    [Fact]
    public async Task Should_Add_Snapshot_And_Add_Variant_To_Attributes_When_All_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId, categoryId: categoryId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId, categoryId: categoryId, name: "Color")
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(
            productVariantId: productVariantId,
            productId: product.Id,
            characteristicValues:
            [
                SnapshotCommandTestsFactory.ValidVariantAttributeDto(characteristicId, value: "Red")
            ]);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        variantSet.Received(1).Add(Arg.Is<CatalogService.Domain.Snapshots.ProductVariantSnapshot>(
            s => s.ProductVariantId == productVariantId && s.ProductId == product.Id));
        product.Attributes.Should().Contain(a => a.CharacteristicId == characteristicId);
    }

    [Fact]
    public async Task Should_Return_Without_Adding_Snapshot_When_Characteristic_GroupName_Is_Invalid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId, categoryId: categoryId);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId, categoryId: categoryId,
                groupName: new string('a', 1000))
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(
            productId: product.Id,
            characteristicValues: [SnapshotCommandTestsFactory.ValidVariantAttributeDto(characteristicId)]);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        variantSet.DidNotReceive().Add(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantSnapshot>());
    }

    [Fact]
    public async Task Should_Log_Warning_And_Still_Add_Snapshot_When_AddVariantToAttributes_Fails()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        var product = ProductTestFactory.CreateArchived();

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();
        var productSet = new List<Product> { product }.BuildMockDbSet();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(product.VendorId)
        }.BuildMockDbSet();
            var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId, categoryId: product.CategoryId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);
        context.Products.Returns(productSet);
        context.VendorSnapshots.Returns(vendorSet);
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddProductVariantSnapshotCommandHandler(
            context, TestClock.Create(), _metrics, _logger);
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantSnapshotCommand(
            productId: product.Id,
            characteristicValues: [SnapshotCommandTestsFactory.ValidVariantAttributeDto(characteristicId)]);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert 
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        variantSet.Received(1).Add(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantSnapshot>());
        _logger.VerifyLog(LogLevel.Warning, new EventId(1004));
    }
}