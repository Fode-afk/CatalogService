using CatalogService.Application.Features.Commands.ArchiveProduct;
using CatalogService.Domain.Errors;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using migApp.Shared.Enums.Products;

namespace CatalogService.IntegrationTests.Application.Features.Commands.ArchiveProduct;

public sealed class ArchiveProductCommandHandlerTests : IAsyncLifetime
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() =>
        _fixture.InitializeAsync();

    public ValueTask DisposeAsync() =>
        _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Archive_Product_When_Request_Is_Valid()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var product = ProductTestFactory.CreateValid(
            vendorId: vendorId);

        var vendorSnapshot = SnapshotTestsFactory.ActiveVendor(
            vendorId);

        _fixture.Context.Products.Add(product);
        _fixture.Context.VendorSnapshots.Add(vendorSnapshot);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var now = new DateTimeOffset(
            2026, 9, 23, 12, 0, 0, TimeSpan.Zero);

        var handler = new ArchiveProductCommandHandler(
            _fixture.Context,
            new FakeTimeProvider(now));

        var command = new ArchiveProductCommand(
            product.Id,
            vendorId);

        // Act
        var result = await handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _fixture.Context.ChangeTracker.Clear();

        var archivedProduct = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .FirstAsync(
                p => p.Id == product.Id,
                _cancellationToken);

        archivedProduct.ProductStatus
            .Should()
            .Be(ProductStatus.Archived);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_VendorSnapshot_Does_Not_Exist()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var product = ProductTestFactory.CreateValid(
            vendorId: vendorId);

        _fixture.Context.Products.Add(product);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var handler = new ArchiveProductCommandHandler(
            _fixture.Context,
            new FakeTimeProvider());

        var command = new ArchiveProductCommand(
            product.Id,
            vendorId);

        // Act
        var result = await handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VendorSnapshotErrors.NotFound());
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var handler = new ArchiveProductCommandHandler(
            _fixture.Context,
            new FakeTimeProvider());

        var command = new ArchiveProductCommand(
            Guid.NewGuid(),
            vendorId);

        // Act
        var result = await handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound());
    }

    [Fact]
    public async Task Should_Return_Failure_When_Product_Belongs_To_Another_Vendor()
    {
        // Arrange
        var productVendorId = Guid.NewGuid();
        var requestVendorId = Guid.NewGuid();

        var product = ProductTestFactory.CreateValid(
            vendorId: productVendorId);

        var vendorSnapshot = SnapshotTestsFactory.ActiveVendor(
            requestVendorId);

        _fixture.Context.Products.Add(product);
        _fixture.Context.VendorSnapshots.Add(vendorSnapshot);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var handler = new ArchiveProductCommandHandler(
            _fixture.Context,
            new FakeTimeProvider());

        var command = new ArchiveProductCommand(
            product.Id,
            requestVendorId);

        // Act
        var result = await handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_Domain_Failure_When_Product_Cannot_Be_Archived()
    {
        // Arrange
        var vendorId = Guid.NewGuid();

        var product = ProductTestFactory.CreateDeleted(vendorId);
        var vendorSnapshot = SnapshotTestsFactory.ActiveVendor(vendorId);

        _fixture.Context.Products.Add(product);
        _fixture.Context.VendorSnapshots.Add(vendorSnapshot);

        await _fixture.Context.SaveChangesAsync(_cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        var handler = new ArchiveProductCommandHandler(
            _fixture.Context,
            new FakeTimeProvider());

        var command = new ArchiveProductCommand(
            product.Id,
            vendorId);

        // Act
        var result = await handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
