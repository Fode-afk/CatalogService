using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductUnlockTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Unlock_ShouldSucceed_WhenProductIsLockedByAdmin()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Unlock_ShouldUnlockProduct()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.IsLockedByAdmin.Should().BeFalse();
    }

    [Fact]
    public void Unlock_ShouldSetProductStatusToDraft()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Unlock_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void Unlock_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        var versionBefore = product.Version;

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Version.Should().Be(versionBefore + 1);
    }

    [Fact]
    public void Unlock_ShouldRaiseProductUnlockedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductUnlockedDomainEvent>();
    }

    [Fact]
    public void Unlock_ShouldRaiseEventWithCurrentState()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        // Act
        var result = product.Unlock(
            ProductContextsTestFactory.ValidUnlockContext(),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var domainEvent = product.DomainEvents
            .Single()
            .Should()
            .BeOfType<ProductUnlockedDomainEvent>()
            .Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.CategoryId.Should().Be(product.CategoryId);
        domainEvent.VendorId.Should().Be(product.VendorId);
        domainEvent.IsLockedByAdmin.Should().BeFalse();
        domainEvent.ProductStatus.Should().Be(ProductStatus.Draft);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void Unlock_ShouldDoNothing_WhenProductIsNotLockedByAdmin()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var statusBefore = product.ProductStatus;
        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        // Act
        var result = product.Unlock(
            new ProductUnlockContext(ProductStatus.Archived),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.IsLockedByAdmin.Should().BeFalse();
        product.ProductStatus.Should().Be(statusBefore);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Unlock_ShouldNotChangeProduct_WhenUnlockIsNotAllowed()
    {
        // Arrange
        var product = ProductTestFactory.CreateLockedByAdmin();

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        var ctx = new ProductUnlockContext(ProductStatus.Archived);

        // Act
        var result = product.Unlock(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.IsLockedByAdmin.Should().BeTrue();
        product.ProductStatus.Should().Be(ProductStatus.Draft);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.DomainEvents.Should().BeEmpty();
    }
}