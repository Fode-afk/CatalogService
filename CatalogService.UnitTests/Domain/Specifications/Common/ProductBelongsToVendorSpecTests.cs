using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class ProductBelongsToVendorSpecTests
{
    private readonly ProductBelongsToVendorSpec _spec = ProductBelongsToVendorSpec.Instance;

    [Fact]
    public void Should_Pass_When_Vendor_Matches()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var ctx = new ProductVendorOwnershipContext(vendorId, vendorId);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Vendor_Does_Not_Match()
    {
        // Arrange
        var ctx = new ProductVendorOwnershipContext(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_With_VendorMismatch_Error()
    {
        // Arrange
        var ctx = new ProductVendorOwnershipContext(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.Error.Should().Be(ProductErrors.VendorMismatch());
    }
}
