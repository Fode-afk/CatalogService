using CatalogService.Application.Features.Commands.UpdateProductInfo;
using CatalogService.Domain.Errors;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CatalogService.UnitTests.Application.Commands.UpdateProductInfo;

public class UpdateProductInfoCommandHandlerTests
{
    [Fact]
    public async Task Should_Update_Product_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
            product.Id, vendorId, categoryId, brandId);

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

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand();

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

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
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
            .WithBrand(SnapshotTestsFactory.AssignableBrand())
            .WithCategory(SnapshotTestsFactory.ActiveCategory())
            .Build();

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_CategorySnapshot_Not_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .Build();

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
            product.Id, vendorId, brandId: brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_BrandSnapshot_Not_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
            product.Id, vendorId, categoryId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_Without_Touching_Snapshots_When_Command_Data_Is_Invalid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
            product.Id, vendorId, categoryId, brandId) with
        { Name = string.Empty };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Update()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.InactiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
            product.Id, vendorId, categoryId, brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_SlugNotUnique_When_SaveChanges_Throws_DbUpdateException()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        context.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new DbUpdateException());

        var handler = new UpdateProductInfoCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidUpdateInfoCommand(
            product.Id, vendorId, categoryId, brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SlugErrors.NotUnique());
    }
}
