using CatalogService.Application.Features.Commands.LockProduct;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.LockProduct;

public class LockProductCommandHandlerTests
{
    [Fact]
    public async Task Should_Lock_Product_When_It_Can_Be_Locked()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new LockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidLockCommand(product.Id);

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

        var handler = new LockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidLockCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Product_Cannot_Be_Modified()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new LockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidLockCommand(product.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Pass_When_Product_Is_Already_Locked()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .Build();

        var handler = new LockProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidLockCommand(product.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}