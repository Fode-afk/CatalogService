using CatalogService.Application.Features.Commands.UnpublishProduct;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.UnpublishProduct;

public class UnpublishProductCommandHandlerTests
{
    [Fact]
    public async Task Should_Unpublish_Product_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(product.VendorId))
            .Build();

        var handler = new UnpublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnpublishCommand(product.Id, product.VendorId);

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

        var handler = new UnpublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnpublishCommand();

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
        var otherVendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new UnpublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnpublishCommand(product.Id, otherVendorId);

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
        var product = ProductTestFactory.CreatePublished();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new UnpublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnpublishCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Unpublish()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.InactiveVendor(product.VendorId))
            .Build();

        var handler = new UnpublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnpublishCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}