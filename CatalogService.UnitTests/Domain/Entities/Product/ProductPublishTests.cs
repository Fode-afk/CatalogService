using CatalogService.Domain.DomainEvents;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductPublishTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Publish_ShouldPublishProduct_WhenContextIsValid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidPublishContext();

        // Act
        var result = product.Publish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Published);
    }

    [Fact]
    public void Publish_ShouldUpdateUpdatedAt_WhenProductIsPublished()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidPublishContext();

        // Act
        var result = product.Publish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void Publish_ShouldIncreaseVersion_WhenProductIsPublished()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidPublishContext();

        var versionBefore = product.Version;

        // Act
        var result = product.Publish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Version.Should().Be(versionBefore + 1);
    }

    [Fact]
    public void Publish_ShouldRaiseProductPublishedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidPublishContext();

        // Act
        var result = product.Publish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductPublishedDomainEvent>();
    }

    [Fact]
    public void Publish_ShouldRaiseEventWithCurrentVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidPublishContext();

        // Act
        product.Publish(ctx, Now);

        // Assert
        var domainEvent = product.DomainEvents
            .Single()
            .Should()
            .BeOfType<ProductPublishedDomainEvent>()
            .Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.CategoryId.Should().Be(product.CategoryId);
        domainEvent.VendorId.Should().Be(product.VendorId);
        domainEvent.ProductStatus.Should().Be(ProductStatus.Published);
        domainEvent.Version.Should().Be(product.Version);
    }
}