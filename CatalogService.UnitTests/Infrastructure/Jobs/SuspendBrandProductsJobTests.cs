using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Models;
using CatalogService.Infrastructure.Jobs;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Infrastructure.Jobs;

public class SuspendBrandProductsJobTests
{
    [Fact(Timeout = 5000)]
    public async Task Should_Suspend_All_Products_When_Brand_Deactivated()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var products = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(brandId: brandId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendBrandProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(brandId, isActive: false, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Restore_All_Products_When_Brand_Reactivated()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var products = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreateSuspended(
                brandId, null, null,
                new ProductSuspensionReason(SuspensionReason.BrandDeactivated)))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendBrandProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(brandId, isActive: true, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus != ProductStatus.Suspended);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Not_Touch_Products_Of_Other_Brands()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var otherBrandId = Guid.NewGuid();

        var ownProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(brandId: brandId))
            .ToList();
        var otherProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(brandId: otherBrandId))
            .ToList();

        var allProducts = ownProducts.Concat(otherProducts).ToList();
        var productSet = allProducts.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendBrandProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(brandId, isActive: false, CancellationToken.None);

        // Assert
        ownProducts.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
        otherProducts.Should().OnlyContain(p => p.ProductStatus != ProductStatus.Suspended);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Do_Nothing_When_Brand_Has_No_Products()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendBrandProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(brandId, isActive: false, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Terminate_Without_Infinite_Loop_When_Suspend_Fails_For_Some_Products()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var unmodifiableProducts = Enumerable.Range(0, 3)
            .Select(_ =>
            {
                var p = ProductTestFactory.CreatePublished(brandId: brandId);
                p.Archive(ProductContextsTestFactory.ValidArchiveContext(), TestClock.DefaultNow);
                return p;
            })
            .ToList();

        var productSet = unmodifiableProducts.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendBrandProductsJob(context, TestClock.Create());

        // Act
        var act = () => job.Execute(brandId, isActive: false, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        unmodifiableProducts.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Archived);
    }
}