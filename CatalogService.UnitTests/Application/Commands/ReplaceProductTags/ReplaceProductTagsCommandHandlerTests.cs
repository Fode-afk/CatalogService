using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.ReplaceProductTags;

public class ReplaceProductTagsCommandHandlerTests
{
    [Fact]
    public async Task Should_Replace_Tags_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .Build();

        var handler = new ReplaceProductTagsCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(product.Id, vendorId);

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

        var handler = new ReplaceProductTagsCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand();

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

        var handler = new ReplaceProductTagsCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(product.Id, otherVendorId);

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

        var handler = new ReplaceProductTagsCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_Without_Touching_Vendor_When_Tags_Are_Invalid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .Build();

        var handler = new ReplaceProductTagsCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(
            product.Id, vendorId, tags: [string.Empty]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Replace()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.InactiveVendor(vendorId))
            .Build();

        var handler = new ReplaceProductTagsCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
