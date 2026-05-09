using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardUpdateInfoSpecificationTests
{
    [Theory]
    [InlineData(false, ProductCardStatus.Draft, true, true, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(false, ProductCardStatus.Published, true, true, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Archived, true, true, ProductErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Draft, false, true, CategorySnapshotErrorCodes.Inactive)]
    [InlineData(true, ProductCardStatus.Draft, true, false, BrandSnapshotErrorCodes.Inactive)]
    [InlineData(false, ProductCardStatus.Archived, false, false, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Draft, true, true, null)]
    [InlineData(true, ProductCardStatus.Published, true, true, null)]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        ProductCardStatus productCardStatus,
        bool categoryIsActive,
        bool brandIsActive,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardUpdateInfoContext(
            vendorIsActive,
            productCardStatus,
            categoryIsActive,
            brandIsActive);

        //Act
        var result = ProductUpdateInfoSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
