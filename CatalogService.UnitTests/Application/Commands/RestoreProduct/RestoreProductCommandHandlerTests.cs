using CatalogService.Application.Features.Commands.RestoreProduct;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.RestoreProduct;

public class RestoreProductCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Restore_Product_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateArchived();
        // ВНИМАНИЕ: CreateArchived() не принимает vendorId — см. замечание ниже

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(product.VendorId))
            .Build();

        var handler = new RestoreProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidRestoreCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.Received(1).RecordProductRestored();
    }

    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        // Arrange
        var context = new AppDbContextTestsBuilder().Build();

        var handler = new RestoreProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidRestoreCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductRestored();
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Does_Not_Own_Product()
    {
        // Arrange
        var otherVendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateArchived();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new RestoreProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidRestoreCommand(product.Id, otherVendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductRestored();
    }

    [Fact]
    public async Task Should_Fail_When_VendorSnapshot_Not_Found()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new RestoreProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidRestoreCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductRestored();
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Restore()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.InactiveVendor(product.VendorId))
            .Build();

        var handler = new RestoreProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidRestoreCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductRestored();
    }

    [Fact]
    public async Task Should_Pass_When_Product_Is_Not_Archived()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(product.VendorId))
            .Build();

        var handler = new RestoreProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidRestoreCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.Received(1).RecordProductRestored();
    }
}