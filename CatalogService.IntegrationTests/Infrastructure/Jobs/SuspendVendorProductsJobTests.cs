using CatalogService.Domain.Models;
using CatalogService.Infrastructure.Jobs;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using migApp.Shared.Enums.Products;

namespace CatalogService.IntegrationTests.Infrastructure.Jobs;

public class SuspendVendorProductsJobTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() =>
        _fixture.InitializeAsync();

    public ValueTask DisposeAsync() =>
        _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Suspend_All_Vendor_Products_When_Vendor_Is_Deactivated()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(
                vendorId: vendorId,
                suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var job = new SuspendVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider(now));

        // Act
        await job.Execute(
            vendorId,
            isActive: false,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.VendorId == vendorId)
            .ToListAsync(_cancellationToken);

        result.Should().HaveCount(3);

        result.Should().OnlyContain(p =>
            p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.VendorDeactivated)));
    }

    [Fact]
    public async Task Should_Restore_All_Vendor_Products_When_Vendor_Is_Activated()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var products = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateSuspended(
                vendorId: vendorId,
                suspensionReasons:
                [
                    new ProductSuspensionReason(
                        SuspensionReason.VendorDeactivated)
                ],
                suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            vendorId,
            isActive: true,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.VendorId == vendorId)
            .ToListAsync(_cancellationToken);

        result.Should().HaveCount(3);

        result.Should().OnlyContain(p =>
            !p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.VendorDeactivated)));
    }

    [Fact]
    public async Task Should_Not_Process_Products_Of_Another_Vendor()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var anotherVendorId = Guid.NewGuid();

        var vendorProducts = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(
                vendorId: vendorId,
                suffix: i.ToString()))
            .ToList();

        var anotherVendorProducts = Enumerable.Range(0, 3)
            .Select(i => ProductTestFactory.CreateValid(
                vendorId: anotherVendorId,
                suffix: i.ToString() + "b"))
            .ToList();

        _fixture.Context.Products.AddRange(
            vendorProducts.Concat(anotherVendorProducts));

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            vendorId,
            isActive: false,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p =>
                p.Id == vendorProducts[0].Id ||
                p.Id == vendorProducts[1].Id ||
                p.Id == vendorProducts[2].Id ||
                p.Id == anotherVendorProducts[0].Id ||
                p.Id == anotherVendorProducts[1].Id ||
                p.Id == anotherVendorProducts[2].Id)
            .ToListAsync(_cancellationToken);

        result
            .Where(p => p.VendorId == vendorId)
            .Should()
            .OnlyContain(p =>
                p.SuspensionReasons.Contains(
                    new ProductSuspensionReason(
                        SuspensionReason.VendorDeactivated)));

        result
            .Where(p => p.VendorId == anotherVendorId)
            .Should()
            .OnlyContain(p =>
                !p.SuspensionReasons.Contains(
                    new ProductSuspensionReason(
                        SuspensionReason.VendorDeactivated)));
    }

    [Fact]
    public async Task Should_Process_Products_In_Batches()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var products = Enumerable.Range(0, 250)
            .Select(i => ProductTestFactory.CreateValid(
                vendorId: vendorId,
                suffix: i.ToString()))
            .ToList();

        _fixture.Context.Products.AddRange(products);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var job = new SuspendVendorProductsJob(
            _fixture.Context,
            new FakeTimeProvider());

        // Act
        await job.Execute(
            vendorId,
            isActive: false,
            _cancellationToken);

        // Assert
        var result = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .Where(p => p.VendorId == vendorId)
            .ToListAsync(_cancellationToken);

        result.Should().HaveCount(250);

        result.Should().OnlyContain(p =>
            p.SuspensionReasons.Contains(
                new ProductSuspensionReason(
                    SuspensionReason.VendorDeactivated)));
    }
}