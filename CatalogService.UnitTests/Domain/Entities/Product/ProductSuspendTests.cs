using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Models;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductSuspendTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Suspend_ShouldSucceed_WhenProductIsPublished()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Suspend_ShouldSetStatusToSuspended()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Suspended);
    }

    [Fact]
    public void Suspend_ShouldAddSuspensionReason()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.SuspensionReasons
            .Should()
            .Contain(reason);
    }

    [Fact]
    public void Suspend_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void Suspend_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var versionBefore = product.Version;

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Version.Should().Be(versionBefore + 1);
    }

    [Fact]
    public void Suspend_ShouldRaiseProductSuspendedDomainEvent()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductSuspendedDomainEvent>();
    }

    [Fact]
    public void Suspend_ShouldRaiseEventWithCurrentState()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var domainEvent = product.DomainEvents
            .Single()
            .Should()
            .BeOfType<ProductSuspendedDomainEvent>()
            .Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.VendorId.Should().Be(product.VendorId);
        domainEvent.ProductStatus.Should().Be(ProductStatus.Suspended);
        domainEvent.SuspensionReasons.Should().Contain(reason);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void Suspend_ShouldDoNothing_WhenProductIsDraft()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var statusBefore = product.ProductStatus;
        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;
        var reasonsBefore = product.SuspensionReasons.ToArray();

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(statusBefore);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.SuspensionReasons.Should().Equal(reasonsBefore);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Suspend_ShouldAddReasonButNotChangeState_WhenAlreadySuspended()
    {
        // Arrange
        var existingReason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);
        var newReason = new ProductSuspensionReason(SuspensionReason.NoPriceAvailable);

        var product = ProductTestFactory.CreateSuspended(null, null, null, existingReason);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        // Act
        var result = product.Suspend(
            ProductContextsTestFactory.ValidSuspendContext(),
            newReason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Suspended);

        product.SuspensionReasons
            .Should()
            .Contain(existingReason);

        product.SuspensionReasons
            .Should()
            .Contain(newReason);

        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Suspend_ShouldFail_WhenProductCannotBeModified()
    {
        // Arrange
        var product = ProductTestFactory.CreatePublished();

        var ctx = new ProductSuspendContext(
            CanBeModified: false);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        // Act
        var result = product.Suspend(
            ctx,
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Published);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.SuspensionReasons.Should().BeEmpty();
        product.DomainEvents.Should().BeEmpty();
    }
}