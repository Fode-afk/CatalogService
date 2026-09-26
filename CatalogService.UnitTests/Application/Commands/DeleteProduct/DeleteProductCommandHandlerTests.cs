using CatalogService.Application.Features.Commands.DeleteProduct;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Delete_Product_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .Build();

        var handler = new DeleteProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidDeleteCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.Received(1).RecordProductDeleted();
    }

    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        // Arrange
        var context = new AppDbContextTestsBuilder().Build();

        var handler = new DeleteProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidDeleteCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductDeleted();
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

        var handler = new DeleteProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidDeleteCommand(product.Id, otherVendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductDeleted();
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

        var handler = new DeleteProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidDeleteCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductDeleted();
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Delete()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.InactiveVendor(vendorId))
            .Build();

        var handler = new DeleteProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidDeleteCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductDeleted();
    }

    [Fact]
    public async Task Should_Fail_When_Product_Cannot_Be_Modified()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(product.VendorId))
            .Build();

        var handler = new DeleteProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidDeleteCommand(product.Id, product.VendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductDeleted();
    }
}
