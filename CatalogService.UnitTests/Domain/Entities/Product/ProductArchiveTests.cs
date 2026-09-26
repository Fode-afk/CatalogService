using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductArchiveTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 9, 14, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Archive_ShouldSetStatusToArchived()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidArchiveContext();

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Archived);
    }

    [Fact]
    public void Archive_ShouldClearSuspensionReasons()
    {
        // Arrange
        var product = ProductTestFactory.CreateSuspended();
        var ctx = ProductContextsTestFactory.ValidArchiveContext();

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.SuspensionReasons.Should().BeEmpty();
    }

    [Fact]
    public void Archive_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidArchiveContext();

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void Archive_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var initialVersion = product.Version;

        var ctx = ProductContextsTestFactory.ValidArchiveContext();

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Archive_ShouldRaiseProductArchivedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidArchiveContext();

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductArchivedDomainEvent>();
    }

    [Fact]
    public void Archive_ShouldRaiseEventWithCurrentState()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidArchiveContext();

        // Act
        product.Archive(ctx, Now);

        // Assert
        var @event = product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductArchivedDomainEvent>()
            .Subject;

        @event.ProductId.Should().Be(product.Id);
        @event.CategoryId.Should().Be(product.CategoryId);
        @event.VendorId.Should().Be(product.VendorId);
        @event.CanBeModified.Should().Be(product.CanBeModified);
        @event.ProductStatus.Should().Be(ProductStatus.Archived);
        @event.IsVisiblePublicly.Should().Be(product.IsVisiblePublicly);
        @event.Version.Should().Be(product.Version);
        @event.SuspensionReasons.Should().BeEmpty();
    }

    [Fact]
    public void Archive_ShouldDoNothing_WhenAlreadyArchived()
    {
        // Arrange
        var product = ProductTestFactory.CreateArchived();

        var initialVersion = product.Version;
        var initialUpdatedAt = product.UpdatedAt;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductArchiveContext(
            VendorIsActive: false,
            CanBeModified: false);

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Archived);
        product.Version.Should().Be(initialVersion);
        product.UpdatedAt.Should().Be(initialUpdatedAt);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }

    [Fact]
    public void Archive_ShouldNotChangeProduct_WhenContextIsInvalid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var initialStatus = product.ProductStatus;
        var initialVersion = product.Version;
        var initialUpdatedAt = product.UpdatedAt;
        var initialEventsCount = product.DomainEvents.Count;

        var ctx = new ProductArchiveContext(
            VendorIsActive: false,
            CanBeModified: true);

        // Act
        var result = product.Archive(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.ProductStatus.Should().Be(initialStatus);
        product.Version.Should().Be(initialVersion);
        product.UpdatedAt.Should().Be(initialUpdatedAt);
        product.DomainEvents.Should().HaveCount(initialEventsCount);
    }
}
