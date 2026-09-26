using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class ProductValueObjectMappingTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();
    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_RoundTrip_Product_Value_Objects_Correctly()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var reloaded = await _fixture.Context.Products
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        reloaded.Name.Value.Should().Be(product.Name.Value);
        reloaded.Slug.Value.Should().Be(product.Slug.Value);
        reloaded.Description.Value.Should().Be(product.Description.Value);
        reloaded.ShortDescription.Value.Should().Be(product.ShortDescription.Value);
        reloaded.ProductStatus.Should().Be(product.ProductStatus);
        reloaded.RowVersion.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_RoundTrip_SeoMetadata_OwnedType()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var reloaded = await _fixture.Context.Products
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        reloaded.SeoMetadata.Title.Value.Should().Be(product.SeoMetadata.Title.Value);
        reloaded.SeoMetadata.Description.Value.Should().Be(product.SeoMetadata.Description.Value);
        reloaded.SeoMetadata.Keywords.Value.Should().Be(product.SeoMetadata.Keywords.Value);
    }

    [Fact]
    public async Task Should_RoundTrip_Tags_OwnedCollection()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(
                [Domain.ValueObjects.Tag.Create("red").Value, Domain.ValueObjects.Tag.Create("sale").Value]),
            DateTimeOffset.UtcNow);

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var reloaded = await _fixture.Context.Products
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        reloaded.Tags.Select(t => t.Value).Should().BeEquivalentTo("red", "sale");
    }

    [Fact]
    public async Task Should_RoundTrip_Attributes_With_VariableValues()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        _fixture.Context.Products.Add(product);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        // Act
        var reloaded = await _fixture.Context.Products
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        // Assert
        reloaded.Attributes.Should().ContainSingle();
        reloaded.Attributes.First().VariableValues.Should()
            .ContainSingle(v => v.ValueId == variantId);
    }
}