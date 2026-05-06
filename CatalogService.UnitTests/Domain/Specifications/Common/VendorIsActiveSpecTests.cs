using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class VendorIsActiveSpecTests
{
    private sealed record TestContext(bool VendorIsActive) : IVendorContext;

    private readonly VendorIsActiveSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Fail_When_Vendor_Is_Not_Active()
    {
        // Arrange
        var ctx = new TestContext(false);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Vendor_Is_Active()
    {
        // Arrange
        var ctx = new TestContext(true);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_CannotModify_Error_When_Inactive()
    {
        // Arrange
        var ctx = new TestContext(false);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.Error.Should().Be(VendorSnapshotErrors.CannotModify());
    }
}
