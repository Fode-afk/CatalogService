using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class ProductQueryFilterTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();
    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Exclude_SoftDeleted_Product_From_Default_Query()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        product.Delete(ProductContextsTestFactory.ValidDeleteContext(), DateTimeOffset.UtcNow);

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var found = await _fixture.Context.Products
            .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task Should_Include_SoftDeleted_Product_When_IgnoreQueryFilters_Used()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        product.Delete(ProductContextsTestFactory.ValidDeleteContext(), DateTimeOffset.UtcNow);

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var found = await _fixture.Context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        found.Should().NotBeNull();
        found!.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Include_NonDeleted_Product_In_Default_Query()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var found = await _fixture.Context.Products
            .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        found.Should().NotBeNull();
    }
}