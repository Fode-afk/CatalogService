using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductRestoreTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 9, 14, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Restore_ShouldSetStatusToDraft()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();
        var ctx = ProductContextsTestFactory.ValidRestoreContext();

        // Act
        var result = product.Restore(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Restore_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();
        var ctx = ProductContextsTestFactory.ValidRestoreContext();

        // Act
        var result = product.Restore(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void Restore_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();
        var initialVersion = product.Version;

        var ctx = ProductContextsTestFactory.ValidRestoreContext();

        // Act
        var result = product.Restore(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Restore_ShouldRaiseProductRestoredDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();
        var ctx = ProductContextsTestFactory.ValidRestoreContext();

        // Act
        var result = product.Restore(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductRestoredDomainEvent>();
    }

    [Fact]
    public void Restore_ShouldRaiseEventWithCurrentState()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();
        var ctx = ProductContextsTestFactory.ValidRestoreContext();

        // Act
        product.Restore(ctx, Now);

        // Assert
        var @event = product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductRestoredDomainEvent>()
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
    public void Restore_ShouldDoNothing_WhenProductIsNotArchived()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var initialStatus = product.ProductStatus;
        var initialVersion = product.Version;
        var initialUpdatedAt = product.UpdatedAt;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductRestoreContext(VendorIsActive: false);

        // Act
        var result = product.Restore(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(initialStatus);
        product.Version.Should().Be(initialVersion);
        product.UpdatedAt.Should().Be(initialUpdatedAt);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }
}
