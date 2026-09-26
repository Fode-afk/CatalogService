using CatalogService.Application.Features.Commands.UnlockProduct;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.UnlockProduct;

public class UnlockProductCommandHandlerTests
{
    [Fact]
    public async Task Should_Unlock_Product_When_It_Is_Locked()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new UnlockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnlockCommand(product.Id);

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

        var handler = new UnlockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnlockCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Pass_When_Product_Is_Not_Locked()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new UnlockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUnlockCommand(product.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}