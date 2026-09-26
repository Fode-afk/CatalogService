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

public class SuspendCategoryProductsJobTests
{
    [Fact(Timeout = 5000)]
    public async Task Should_Suspend_All_Products_When_Category_Deactivated()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var products = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(categoryId: categoryId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendCategoryProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(categoryId, isActive: false, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Restore_All_Products_When_Category_Reactivated()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var products = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreateSuspended(
                null, categoryId, null, new ProductSuspensionReason(SuspensionReason.CategoryDeactivated)))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendCategoryProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(categoryId, isActive: true, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus != ProductStatus.Suspended);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Not_Touch_Products_Of_Other_Categories()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();

        var ownProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(categoryId: categoryId))
            .ToList();
        var otherProducts = Enumerable.Range(0, 3)
            .Select(_ => ProductTestFactory.CreatePublished(categoryId: otherCategoryId))
            .ToList();

        var allProducts = ownProducts.Concat(otherProducts).ToList();
        var productSet = allProducts.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendCategoryProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(categoryId, isActive: false, CancellationToken.None);

        // Assert
        ownProducts.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
        otherProducts.Should().OnlyContain(p => p.ProductStatus != ProductStatus.Suspended);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Do_Nothing_When_Category_Has_No_Products()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var productSet = new List<Product>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendCategoryProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(categoryId, isActive: false, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_Suspend_All_Products_When_Count_Exceeds_BatchSize()
    {
        // Arrange 
        var categoryId = Guid.NewGuid();
        var products = Enumerable.Range(0, 250)
            .Select(_ => ProductTestFactory.CreatePublished(categoryId: categoryId))
            .ToList();

        var productSet = products.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.Products.Returns(productSet);

        var job = new SuspendCategoryProductsJob(context, TestClock.Create());

        // Act
        await job.Execute(categoryId, isActive: false, CancellationToken.None);

        // Assert
        products.Should().OnlyContain(p => p.ProductStatus == ProductStatus.Suspended);
    }
}
