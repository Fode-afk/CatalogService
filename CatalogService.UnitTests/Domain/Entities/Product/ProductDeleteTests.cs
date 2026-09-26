using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductDeleteTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 9, 14, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Delete_ShouldSetDeletedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidDeleteContext();

        // Act
        var result = product.Delete(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.DeletedAt.Should().Be(Now);
    }

    [Fact]
    public void Delete_ShouldChangePublishedProductToDraft()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();
        var ctx = ProductContextsTestFactory.ValidDeleteContext();

        // Act
        var result = product.Delete(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Delete_ShouldNotChangeStatus_WhenProductIsNotPublished()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var initialStatus = product.ProductStatus;
        var ctx = ProductContextsTestFactory.ValidDeleteContext();

        // Act
        var result = product.Delete(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(initialStatus);
    }

    [Fact]
    public void Delete_ShouldRaiseProductDeletedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidDeleteContext();

        // Act
        var result = product.Delete(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_ShouldRaiseEventWithProductId()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidDeleteContext();

        // Act
        product.Delete(ctx, Now);

        // Assert
        var @event = product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductDeletedDomainEvent>()
            .Subject;

        @event.ProductId.Should().Be(product.Id);
    }

    [Fact]
    public void Delete_ShouldDoNothing_WhenAlreadyDeleted()
    {
        // Arrange
        var product = ProductTestFactory.CreateDeleted();

        var initialDeletedAt = product.DeletedAt;
        var initialStatus = product.ProductStatus;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductDeleteContext(
            VendorIsActive: false,
            CanBeModified: false);

        // Act
        var result = product.Delete(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DeletedAt.Should().Be(initialDeletedAt);
        product.ProductStatus.Should().Be(initialStatus);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }

    [Fact]
    public void Delete_ShouldNotModifyProduct_WhenContextIsInvalid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var initialDeletedAt = product.DeletedAt;
        var initialStatus = product.ProductStatus;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductDeleteContext(
            VendorIsActive: false,
            CanBeModified: true);

        // Act
        var result = product.Delete(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.DeletedAt.Should().Be(initialDeletedAt);
        product.ProductStatus.Should().Be(initialStatus);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }

    [Fact]
    public void ForceDelete_ShouldSetDeletedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        // Act
        var result = product.ForceDelete(Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.DeletedAt.Should().Be(Now);
    }

    [Fact]
    public void ForceDelete_ShouldChangePublishedProductToDraft()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        // Act
        var result = product.ForceDelete(Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void ForceDelete_ShouldRaiseProductDeletedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        // Act
        var result = product.ForceDelete(Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductDeletedDomainEvent>();
    }

    [Fact]
    public void ForceDelete_ShouldIgnoreDeleteContextRestrictions()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        // Act
        var result = product.ForceDelete(Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.IsDeleted.Should().BeTrue();
        product.DeletedAt.Should().Be(Now);
    }

    [Fact]
    public void ForceDelete_ShouldDoNothing_WhenAlreadyDeleted()
    {
        // Arrange
        var product = ProductTestFactory.CreateDeleted();

        var initialDeletedAt = product.DeletedAt;
        var initialStatus = product.ProductStatus;
        var initialEventsCount = product.DomainEvents.Count;

        // Act
        var result = product.ForceDelete(Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DeletedAt.Should().Be(initialDeletedAt);
        product.ProductStatus.Should().Be(initialStatus);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }
}
