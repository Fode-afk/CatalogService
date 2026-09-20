using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Domain.Models;
using CatalogService.Infrastructure.Jobs;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Infrastructure.Jobs;

public class DeleteVendorProductsJobTests
{
    [Fact]
    public async Task Should_Delete_All_Products_When_Count_Is_Below_BatchSize()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var products = Enumerable.Range(0, 5)
            .Select(_ => ProductTestFactory.CreateValid(vendorId: vendorId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new DeleteVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.DeletedAt != null);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Vendor_Has_No_Products()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new DeleteVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Touch_Products_Of_Other_Vendors()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var otherVendorId = Guid.NewGuid();

        var ownProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreateValid(vendorId: vendorId))
            .ToList();
        var otherProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreateValid(vendorId: otherVendorId))
            .ToList();

        var allProducts = ownProducts.Concat(otherProducts).ToList();
        var productSet = allProducts.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new DeleteVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, CancellationToken.None);

        // Assert
        ownProducts.Should().OnlyContain(p => p.DeletedAt != null);
        otherProducts.Should().OnlyContain(p => p.DeletedAt == null);
    }

    [Fact]
    public async Task Should_Not_Touch_Already_Deleted_Products()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var activeProduct = ProductTestFactory.CreateValid(vendorId: vendorId);
        var alreadyDeletedProduct = ProductTestFactory.CreateDeleted();

        var productSet = new List<Product> { activeProduct, alreadyDeletedProduct }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new DeleteVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, CancellationToken.None);

        // Assert
        activeProduct.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Delete_All_Products_When_Count_Exceeds_BatchSize()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var products = Enumerable.Range(0, 200)
            .Select(_ => ProductTestFactory.CreateValid(vendorId: vendorId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new DeleteVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.DeletedAt != null,
            "all vendor products should be force-deleted regardless of batch count");
    }
}
