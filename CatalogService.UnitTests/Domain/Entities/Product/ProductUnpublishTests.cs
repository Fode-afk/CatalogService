using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductUnpublishTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 9, 14, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Unpublish_ShouldSetStatusToDraft()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();
        var ctx = ProductContextsTestFactory.ValidUnpublishContext();

        // Act
        var result = product.Unpublish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Unpublish_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();
        var ctx = ProductContextsTestFactory.ValidUnpublishContext();

        // Act
        var result = product.Unpublish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void Unpublish_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();
        var initialVersion = product.Version;

        var ctx = ProductContextsTestFactory.ValidUnpublishContext();

        // Act
        var result = product.Unpublish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Unpublish_ShouldRaiseProductUnpublishedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();
        var ctx = ProductContextsTestFactory.ValidUnpublishContext();

        // Act
        var result = product.Unpublish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductUnpublishedDomainEvent>();
    }

    [Fact]
    public void Unpublish_ShouldRaiseEventWithCurrentState()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();
        var ctx = ProductContextsTestFactory.ValidUnpublishContext();

        // Act
        product.Unpublish(ctx, Now);

        // Assert
        var @event = product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductUnpublishedDomainEvent>()
            .Subject;

        @event.ProductId.Should().Be(product.Id);
        @event.CategoryId.Should().Be(product.CategoryId);
        @event.VendorId.Should().Be(product.VendorId);
        @event.CanBeModified.Should().Be(product.CanBeModified);
        @event.ProductStatus.Should().Be(ProductStatus.Draft);
        @event.IsVisiblePublicly.Should().Be(product.IsVisiblePublicly);
        @event.Version.Should().Be(product.Version);
    }

    [Fact]
    public void Unpublish_ShouldDoNothing_WhenProductIsNotPublished()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var initialStatus = product.ProductStatus;
        var initialVersion = product.Version;
        var initialUpdatedAt = product.UpdatedAt;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductUnpublishContext(
            VendorIsActive: false,
            CanBeModified: false);

        // Act
        var result = product.Unpublish(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(initialStatus);
        product.Version.Should().Be(initialVersion);
        product.UpdatedAt.Should().Be(initialUpdatedAt);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }

    [Fact]
    public void Unpublish_ShouldNotModifyProduct_WhenContextIsInvalid()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var initialStatus = product.ProductStatus;
        var initialVersion = product.Version;
        var initialUpdatedAt = product.UpdatedAt;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductUnpublishContext(
            VendorIsActive: false,
            CanBeModified: true);

        // Act
        var result = product.Unpublish(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.ProductStatus.Should().Be(initialStatus);
        product.Version.Should().Be(initialVersion);
        product.UpdatedAt.Should().Be(initialUpdatedAt);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }
}