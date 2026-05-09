using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardChangeImageOrderSpecificationTests
{
    [Theory]
    [InlineData(false, ProductCardStatus.Draft, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Archived, ProductErrorCodes.CannotModify)]
    [InlineData(false, ProductCardStatus.Archived, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, ProductCardStatus.Draft, null)]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        ProductCardStatus status,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardChangeImageOrderContext(
            vendorActive,
            status);

        //Act
        var result = ProductCardChangeImageOrderSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
