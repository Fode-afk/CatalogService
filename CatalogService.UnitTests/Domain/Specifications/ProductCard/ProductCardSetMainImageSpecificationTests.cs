using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardSetMainImageSpecificationTests
{
    [Theory]
    [InlineData(false, ProductCardStatus.Draft, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(false, ProductCardStatus.Published, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Archived, ProductErrorCodes.CannotModify)]
    [InlineData(false, ProductCardStatus.Archived, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Draft, null)]
    [InlineData(true, ProductCardStatus.Published, null)]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        ProductCardStatus productCardStatus,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardSetMainImageContext(
            vendorActive,
            productCardStatus);

        //Act
        var result = ProductCardSetMainImageSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
