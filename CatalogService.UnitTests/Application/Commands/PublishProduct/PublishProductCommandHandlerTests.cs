using CatalogService.Application.Features.Commands.PublishProduct;
using CatalogService.Domain.Snapshots;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace CatalogService.UnitTests.Application.Commands.PublishProduct;

public class PublishProductCommandHandlerTests
{
    [Fact]
    public async Task Should_Publish_Product_When_All_References_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var product = ProductTestFactory.CreatePublished(
            variantId: variantId,
            vendorId: vendorId, 
            categoryId: categoryId,
            brandId: brandId);
        product.Unpublish(ProductContextsTestFactory.ValidUnpublishContext(), TestClock.DefaultNow);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(categoryId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(brandId))
            .WithVariant(new ProductVariantSnapshot
            {
                ProductId = product.Id,
                ProductVariantId = variantId,
                HasMainImage = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            })
            .WithPrice(new ProductVariantPriceSnapshot
            {
                ProductId = product.Id,
                ProductVariantId = variantId,
                HasPrice = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            })   
            .Build();

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, vendorId);

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

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand();

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

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, otherVendorId);

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

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, vendorId);

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
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .Build();

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, vendorId);

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
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(product.CategoryId))
            .Build();

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Product_Has_No_Variations()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.ActiveVendor(vendorId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(product.CategoryId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(product.BrandId))
            .Build();

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Vendor_Is_Inactive_And_Domain_Rejects_Publish()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var product = ProductTestFactory.CreateValid(vendorId: vendorId);
        var variantId = Guid.NewGuid();

        var context = new AppDbContextTestsBuilder()
            .WithProducts(product)
            .WithVendor(SnapshotTestsFactory.InactiveVendor(vendorId))
            .WithCategory(SnapshotTestsFactory.ActiveCategory(product.CategoryId))
            .WithBrand(SnapshotTestsFactory.AssignableBrand(product.BrandId))
            .WithVariant(new ProductVariantSnapshot
            {
                ProductId = product.Id,
                ProductVariantId = variantId,
                HasMainImage = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            })
            .Build();

        var handler = new PublishProductCommandHandler(context, TestClock.Create());
        var command = ProductCommandTestsFactory.ValidPublishCommand(product.Id, vendorId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}