using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.ReplaceProductAttributes;

public class ReplaceProductAttributesCommandHandlerTests
{
    [Fact]
    public async Task Should_Replace_Attributes_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var characteristicId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId, categoryId: categoryId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCharacteristic(SnapshotTestsFactory.Characteristic(
                characteristicId, categoryId: categoryId, isUnifying: true))
            .Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            product.Id,
            vendorId,
            attributes: new Dictionary<Guid, string> { [characteristicId] = "Red" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        // Arrange
        var context = new AppDbContextTestsBuilder().Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Does_Not_Own_Product()
    {
        // Arrange
        var actualVendorId = Guid.NewGuid();
        var otherVendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: actualVendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            product.Id, otherVendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_VendorSnapshot_Not_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_No_Matching_CharacteristicSnapshots_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Only_Some_Requested_Characteristics_Are_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var foundCharacteristicId = Guid.NewGuid();
        var missingCharacteristicId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId, categoryId: categoryId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCharacteristic(SnapshotTestsFactory.Characteristic(
                foundCharacteristicId, categoryId: categoryId))
            .Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            product.Id,
            vendorId,
            attributes: new Dictionary<Guid, string>
            {
                [foundCharacteristicId] = "Red",
                [missingCharacteristicId] = "Blue"
            });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_CharacteristicSnapshot_Belongs_To_Different_Category()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);
        var characteristicId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCharacteristic(SnapshotTestsFactory.Characteristic(
                characteristicId, categoryId: Guid.NewGuid()))
            .Build();

        var handler = new ReplaceProductAttributesCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            product.Id, vendorId,
            attributes: new Dictionary<Guid, string> { [characteristicId] = "Red" });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
