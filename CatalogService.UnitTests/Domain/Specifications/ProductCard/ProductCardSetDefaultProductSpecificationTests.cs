using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.ProductCard;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardSetDefaultProductSpecificationTests
{
    [Theory]
    [InlineData(false, ProductCardStatus.Draft, true, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(false, ProductCardStatus.Published, true, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Archived, true, ProductCardErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Draft, false, ProductCardErrorCodes.ProductDoesNotBelongToCard)]
    [InlineData(false, ProductCardStatus.Archived, false, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Draft, true, null)]
    [InlineData(true, ProductCardStatus.Published, true, null)]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        ProductCardStatus productCardStatus,
        bool defaultProductBelongsToCard,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardSetDefaultProductContext(
            vendorActive,
            productCardStatus,
            defaultProductBelongsToCard);

        //Act
        var result = ProductCardSetDefaultProductSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
