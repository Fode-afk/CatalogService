using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Models;
using CatalogService.Infrastructure.BackgroundServices;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Infrastructure.BackgroundServices;

public class SoftDeletedProductsCleanupServiceTests
{
    private readonly ILogger<SoftDeletedProductsCleanupService> _logger =
        Substitute.For<ILogger<SoftDeletedProductsCleanupService>>();

    public SoftDeletedProductsCleanupServiceTests()
    {
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
    }

    [Fact]
    public async Task Should_Delete_Products_Older_Than_RetentionPeriod()
    {
        // Arrange
        var deletedAt = TestClock.DefaultNow - TimeSpan.FromDays(40);
        var product = ProductTestFactory.CreateValid();
        product.Delete(ProductContextsTestFactory.ValidDeleteContext(), deletedAt);

        var products = new List<Product> { product };
        var context = Substitute.For<IAppDbContext>();
        var mockProducts = products.BuildMockDbSet();
        context.Products.Returns(mockProducts);

        var scopeFactory = ServiceScopeFactoryTestsHelper.CreateFor(context);
        var options = ServiceScopeFactoryTestsHelper.ValidCleanupOptions(retentionPeriod: TimeSpan.FromDays(30));

        var service = new SoftDeletedProductsCleanupService(
            scopeFactory, TestClock.Create(), options, _logger);

        // Act
        await service.CleanupAsync(CancellationToken.None);

        // Assert
        products.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Not_Delete_Products_Within_RetentionPeriod()
    {
        // Arrange
        var deletedAt = TestClock.DefaultNow - TimeSpan.FromDays(5);
        var product = ProductTestFactory.CreateValid();
        product.Delete(ProductContextsTestFactory.ValidDeleteContext(), deletedAt);

        var products = new List<Product> { product };
        var context = Substitute.For<IAppDbContext>();
        var mockProducts = products.BuildMockDbSet();
        context.Products.Returns(mockProducts);

        var scopeFactory = ServiceScopeFactoryTestsHelper.CreateFor(context);
        var options = ServiceScopeFactoryTestsHelper.ValidCleanupOptions(retentionPeriod: TimeSpan.FromDays(30));

        var service = new SoftDeletedProductsCleanupService(
            scopeFactory, TestClock.Create(), options, _logger);

        // Act
        await service.CleanupAsync(CancellationToken.None);

        // Assert
        products.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_Not_Delete_Non_Deleted_Products()
    {
        // Arrange
        var activeProduct = ProductTestFactory.CreateValid();

        var products = new List<Product> { activeProduct };
        var context = Substitute.For<IAppDbContext>();
        var mockProducts = products.BuildMockDbSet();
        context.Products.Returns(mockProducts);

        var scopeFactory = ServiceScopeFactoryTestsHelper.CreateFor(context);
        var options = ServiceScopeFactoryTestsHelper.ValidCleanupOptions();

        var service = new SoftDeletedProductsCleanupService(
            scopeFactory, TestClock.Create(), options, _logger);

        // Act
        await service.CleanupAsync(CancellationToken.None);

        // Assert
        products.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_Do_Nothing_When_No_Products_To_Delete()
    {
        // Arrange
        var context = Substitute.For<IAppDbContext>();
        var mockProducts = new List<Product>().BuildMockDbSet();
        context.Products.Returns(mockProducts);

        var scopeFactory = ServiceScopeFactoryTestsHelper.CreateFor(context);
        var options = ServiceScopeFactoryTestsHelper.ValidCleanupOptions();

        var service = new SoftDeletedProductsCleanupService(
            scopeFactory, TestClock.Create(), options, _logger);

        // Act
        var act = () => service.CleanupAsync(CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Should_Delete_All_Eligible_Products_Across_Multiple_Batches()
    {
        // Arrange
        var now = TestClock.DefaultNow;
        var oldProducts = Enumerable.Range(0, 5)
            .Select(_ =>
            {
                var deletedAt = TestClock.DefaultNow - TimeSpan.FromDays(40);
                var product = ProductTestFactory.CreateValid();
                product.Delete(ProductContextsTestFactory.ValidDeleteContext(), deletedAt);
                return product;
            })
            .ToList();

        var context = Substitute.For<IAppDbContext>();
        var mockProducts = oldProducts.BuildMockDbSet();
        context.Products.Returns(mockProducts);

        var scopeFactory = ServiceScopeFactoryTestsHelper.CreateFor(context);
        var options = ServiceScopeFactoryTestsHelper.ValidCleanupOptions(
            retentionPeriod: TimeSpan.FromDays(30), batchSize: 2);

        var service = new SoftDeletedProductsCleanupService(
            scopeFactory, TestClock.Create(), options, _logger);

        // Act
        await service.CleanupAsync(CancellationToken.None);

        // Assert
        oldProducts.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Not_Throw_When_Cancellation_Is_Requested()
    {
        // Arrange
        var context = Substitute.For<IAppDbContext>();
        var mockProducts = new List<Product>().BuildMockDbSet();
        context.Products.Returns(mockProducts);

        var scopeFactory = ServiceScopeFactoryTestsHelper.CreateFor(context);
        var options = ServiceScopeFactoryTestsHelper.ValidCleanupOptions();

        var service = new SoftDeletedProductsCleanupService(
            scopeFactory, TestClock.Create(), options, _logger);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = () => service.CleanupAsync(cts.Token);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
