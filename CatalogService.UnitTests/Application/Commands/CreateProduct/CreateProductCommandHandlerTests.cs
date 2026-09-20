using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Errors;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CatalogService.UnitTests.Application.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Create_Product_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand(vendorId, categoryId, brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _metrics.Received(1).RecordProductCreated();
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Not_Found()
    {
        // Arrange
        var context = new AppDbContextTestsBuilder()
            .WithBrand(SnapshotTestsFactory.AssignableBrand())
            .WithCategory(SnapshotTestsFactory.ActiveCategory())
            .Build();

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VendorSnapshotErrors.NotFound());
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductCreated();
    }

    [Fact]
    public async Task Should_Fail_When_Brand_Not_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand(vendorId, categoryId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BrandSnapshotErrors.NotFound());
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductCreated();
    }

    [Fact]
    public async Task Should_Fail_When_Category_Not_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .Build();

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand(vendorId, brandId: brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategorySnapshotErrors.NotFound());
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Creation()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithVendor(SnapshotTestsFactory.InactiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand(vendorId, categoryId, brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductCreated();
    }

    [Fact]
    public async Task Should_Fail_Without_Touching_References_When_Command_Data_Is_Invalid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand(vendorId, categoryId, brandId)
            with
        { Name = string.Empty };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _metrics.DidNotReceive().RecordProductCreated();
    }

    [Fact]
    public async Task Should_Return_AlreadyExists_When_SaveChanges_Throws_DbUpdateException()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .Build();

        context.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new DbUpdateException());

        var handler = new CreateProductCommandHandler(context, _metrics, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidCreateCommand(vendorId, categoryId, brandId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.AlreadyExists());
        _metrics.DidNotReceive().RecordProductCreated();
    }
}
