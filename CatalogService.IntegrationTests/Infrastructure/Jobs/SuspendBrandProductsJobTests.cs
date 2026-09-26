using CatalogService.Domain.Models;
using CatalogService.Infrastructure.Jobs;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using migApp.Shared.Enums.Products;

namespace CatalogService.IntegrationTests.Infrastructure.Jobs;

public class SuspendBrandProductsJobTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Suspend_All_Brand_Products_When_Brand_Is_Deactivated()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(brandId: brandId, suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new SuspendBrandProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(
            brandId,
            isActive: false,
            cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.BrandId == brandId)
            .ToListAsync(cancellationToken);

        result.Should().HaveCount(3);

        result.Should().OnlyContain(p =>
            p.ProductStatus == ProductStatus.Suspended &&
            p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.BrandDeactivated)));
    }

    [Fact]
    public async Task Should_Restore_Brand_Products_When_Brand_Is_Activated()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i =>
                ProductTestFactory.CreateSuspended(
                    brandId: brandId,
                    suspensionReasons:
                    [
                        new ProductSuspensionReason(
                            SuspensionReason.BrandDeactivated)
                    ],
                    suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new SuspendBrandProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(
            brandId,
            isActive: true,
            cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.BrandId == brandId)
            .ToListAsync(cancellationToken);

        result.Should().HaveCount(3);

        result.Should().OnlyContain(p =>
            !p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.BrandDeactivated)));
    }

    [Fact]
    public async Task Should_Not_Process_Products_Of_Another_Brand()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var anotherBrandId = Guid.NewGuid();

        var brandProducts = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(brandId: brandId, suffix: i.ToString()))
            .ToList();

        var anotherBrandProducts = Enumerable.Range(0, 3)
            .Select(_ =>
                ProductTestFactory.CreateValid(
                    brandId: anotherBrandId))
            .ToList();

        _fixture.Context.Products.AddRange(
            brandProducts.Concat(anotherBrandProducts));

        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendBrandProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            brandId,
            isActive: false,
            cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p =>
                p.Id == brandProducts[0].Id ||
                p.Id == brandProducts[1].Id ||
                p.Id == brandProducts[2].Id ||
                p.Id == anotherBrandProducts[0].Id ||
                p.Id == anotherBrandProducts[1].Id ||
                p.Id == anotherBrandProducts[2].Id)
            .ToListAsync(cancellationToken);

        result
            .Where(p => p.BrandId == brandId)
            .Should()
            .OnlyContain(p =>
                p.SuspensionReasons.Contains(
                    new ProductSuspensionReason(
                        SuspensionReason.BrandDeactivated)));

        result
            .Where(p => p.BrandId == anotherBrandId)
            .Should()
            .OnlyContain(p =>
                !p.SuspensionReasons.Contains(
                    new ProductSuspensionReason(
                        SuspensionReason.BrandDeactivated)));
    }

    [Fact]
    public async Task Should_Process_Products_In_Batches()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var products = Enumerable.Range(0, 250)
            .Select(i => ProductTestFactory.CreateValid(brandId: brandId, suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendBrandProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            brandId,
            isActive: false,
            cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.BrandId == brandId)
            .ToListAsync(cancellationToken);

        result.Should().HaveCount(250);

        result.Should().OnlyContain(p =>
            p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.BrandDeactivated)));
    }
}