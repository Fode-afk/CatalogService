using CatalogService.Domain.Models;
using CatalogService.Infrastructure.Jobs;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using migApp.Shared.Enums.Products;

namespace CatalogService.IntegrationTests.Infrastructure.Jobs;

public class SuspendCategoryProductsJobTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Suspend_All_Category_Products_When_Category_Is_Deactivated()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(
                categoryId: categoryId, 
                suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new SuspendCategoryProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(
            categoryId,
            isActive: false,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(_cancellationToken);

        result.Should().HaveCount(3);

        result.Should().OnlyContain(p =>
            p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.CategoryDeactivated)));
    }

    [Fact]
    public async Task Should_Restore_All_Category_Products_When_Category_Is_Activated()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateSuspended(
                categoryId: categoryId,
                suspensionReasons:
                [
                    new ProductSuspensionReason(
                        SuspensionReason.CategoryDeactivated)
                ],
                suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendCategoryProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            categoryId,
            isActive: true,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(_cancellationToken);

        result.Should().HaveCount(3);

        result.Should().OnlyContain(p =>
            !p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.CategoryDeactivated)));
    }

    [Fact]
    public async Task Should_Not_Process_Products_Of_Another_Category()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var anotherCategoryId = Guid.NewGuid();

        var categoryProducts = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(
                categoryId: categoryId,
                suffix: i.ToString()))
            .ToList();

        var anotherCategoryProducts = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(
                categoryId: anotherCategoryId,
                suffix: i.ToString() + "b"))
            .ToList();

        _fixture.Context.Products.AddRange(
            categoryProducts.Concat(anotherCategoryProducts));

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendCategoryProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            categoryId,
            isActive: false,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p =>
                p.Id == categoryProducts[0].Id ||
                p.Id == categoryProducts[1].Id ||
                p.Id == categoryProducts[2].Id ||
                p.Id == anotherCategoryProducts[0].Id ||
                p.Id == anotherCategoryProducts[1].Id ||
                p.Id == anotherCategoryProducts[2].Id)
            .ToListAsync(_cancellationToken);

        result
            .Where(p => p.CategoryId == categoryId)
            .Should()
            .OnlyContain(p =>
                p.SuspensionReasons.Contains(
                    new ProductSuspensionReason(
                        SuspensionReason.CategoryDeactivated)));

        result
            .Where(p => p.CategoryId == anotherCategoryId)
            .Should()
            .OnlyContain(p =>
                !p.SuspensionReasons.Contains(
                    new ProductSuspensionReason(
                        SuspensionReason.CategoryDeactivated)));
    }

    [Fact]
    public async Task Should_Process_Products_In_Batches()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var products = Enumerable.Range(0, 250)
            .Select(i => ProductTestFactory.CreateValid(
                categoryId: categoryId,
                suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendCategoryProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            categoryId,
            isActive: false,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(_cancellationToken);

        result.Should().HaveCount(250);

        result.Should().OnlyContain(p =>
            p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.CategoryDeactivated)));
    }
}