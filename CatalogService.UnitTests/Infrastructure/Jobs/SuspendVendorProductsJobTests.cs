using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Models;
using CatalogService.Infrastructure.Jobs;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Infrastructure.Jobs;

public class SuspendVendorProductsJobTests
{
    [Fact(Timeout = 5000)]
    public async Task Should_Suspend_All_Products_When_Vendor_Deactivated()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var products = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(vendorId: vendorId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, isActive: false, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Restore_All_Products_When_Vendor_Reactivated()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var products = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreateSuspended(null, null, vendorId,
                new ProductSuspensionReason(SuspensionReason.VendorDeactivated)))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, isActive: true, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus != ProductStatus.Suspended);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Not_Touch_Products_Of_Other_Vendors()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var otherVendorId = Guid.NewGuid();

        var ownProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(vendorId: vendorId))
            .ToList();
        var otherProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(vendorId: otherVendorId))
            .ToList();

        var allProducts = ownProducts.Concat(otherProducts).ToList();
        var productSet = allProducts.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, isActive: false, CancellationToken.None);

        // Assert
        ownProducts.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
        otherProducts.Should().OnlyContain(p => p.ProductStatus != ProductStatus.Suspended);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Do_Nothing_When_Vendor_Has_No_Products()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, isActive: false, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Suspend_All_Products_When_Count_Exceeds_BatchSize()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var products = Enumerable.Range(0, 250)
            .Select(_ => ProductTestFactory.CreatePublished(vendorId: vendorId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendVendorProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(vendorId, isActive: false, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
    }
}