using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Models;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductTryRestoreTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void TryRestore_ShouldSucceed_WhenProductIsSuspended()
    {
        // Arrange
        var product = ProductTestFactory.CreateSuspended(null, null, null,
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated));

        product.ClearDomainEvents();

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            new ProductSuspensionReason(SuspensionReason.VendorDeactivated),
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryRestore_ShouldRemoveSuspensionReason()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.SuspensionReasons
            .Should()
            .NotContain(reason);
    }

    [Fact]
    public void TryRestore_ShouldKeepProductSuspended_WhenOtherReasonsRemain()
    {
        // Arrange
        var reasonToRemove = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);
        var remainingReason = new ProductSuspensionReason(SuspensionReason.NoPriceAvailable);

        var product = ProductTestFactory.CreateSuspended(
            null, null, null, [reasonToRemove,
            remainingReason]);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reasonToRemove,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Suspended);

        product.SuspensionReasons
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(remainingReason);

        product.Version.Should().Be(versionBefore + 1);
        product.UpdatedAt.Should().Be(Now);
        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductUnsuspendedDomainEvent>();
    }

    [Fact]
    public void TryRestore_ShouldRestoreProductToDraft_WhenLastReasonIsRemoved()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Draft);
        product.SuspensionReasons.Should().BeEmpty();
    }

    [Fact]
    public void TryRestore_ShouldUpdateUpdatedAt_WhenLastReasonIsRemoved()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void TryRestore_ShouldIncreaseVersion_WhenLastReasonIsRemoved()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        var versionBefore = product.Version;

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Version.Should().Be(versionBefore + 1);
    }

    [Fact]
    public void TryRestore_ShouldRaiseProductUnsuspendedDomainEvent_WhenLastReasonIsRemoved()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        product.ClearDomainEvents();

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductUnsuspendedDomainEvent>();
    }

    [Fact]
    public void TryRestore_ShouldRaiseEventWithCurrentState()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        product.ClearDomainEvents();

        // Act
        var result = product.TryRestore(
            ProductContextsTestFactory.ValidTryRestoreContext(),
            reason,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var domainEvent = product.DomainEvents
            .Single()
            .Should()
            .BeOfType<ProductUnsuspendedDomainEvent>()
            .Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.VendorId.Should().Be(product.VendorId);
        domainEvent.ProductStatus.Should().Be(ProductStatus.Draft);
        domainEvent.SuspensionReasons.Should().BeEmpty();
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void TryRestore_ShouldDoNothing_WhenProductIsNotSuspended()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var statusBefore = product.ProductStatus;
        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;
        var reasonsBefore = product.SuspensionReasons.ToArray();

        // Act
        var result = product.TryRestore(
            new ProductTryRestoreContext(ProductStatus.Archived),
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
    public void TryRestore_ShouldNotRemoveReason_WhenRestoreIsNotAllowed()
    {
        // Arrange
        var reason = new ProductSuspensionReason(SuspensionReason.VendorDeactivated);

        var product = ProductTestFactory.CreateSuspended(null, null, null, reason);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;

        var ctx = new ProductTryRestoreContext(
            ProductStatus.Archived);

        // Act
        var result = product.TryRestore(
            ctx,
            reason,
            Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.ProductStatus.Should().Be(ProductStatus.Suspended);
        product.SuspensionReasons.Should().Contain(reason);
        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.DomainEvents.Should().BeEmpty();
    }
}
