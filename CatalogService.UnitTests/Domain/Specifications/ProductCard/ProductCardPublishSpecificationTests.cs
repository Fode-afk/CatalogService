using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.ProductCard;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardPublishSpecificationTests
{
    public static TheoryData<bool, bool, bool, bool, ProductCardStatus, int, string?> TestCases =>
       new()
       {
            { false, true, true, true, ProductCardStatus.Draft, 1, VendorSnapshotErrorCodes.CannotModify },
            { true, false, true, true, ProductCardStatus.Draft, 1, ProductCardErrorCodes.NoDefaultProduct },
            { true, true, false, true, ProductCardStatus.Draft, 1, ProductPriceSnapshotErrorCodes.NoPrice },
            { true, true, true, false, ProductCardStatus.Draft, 1, ProductInventorySnapshotErrorCodes.OutOfStock },
            { true, true, true, true, ProductCardStatus.Archived, 1, ProductCardErrorCodes.CannotModify },
            { true, true, true, true, ProductCardStatus.Draft, 0, ProductCardErrorCodes.ImagesRequired },
            { false, false, false, false, ProductCardStatus.Archived, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, true, true, true, ProductCardStatus.Draft, 1, null },
            { true, true, true, true, ProductCardStatus.Published, 1, null }
       };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        bool hasDefaultProduct,
        bool defaultProductHasPrice,
        bool defaultProductInStock,
        ProductCardStatus productCardStatus,
        int imagesCount,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardPublishContext(
            vendorActive,
            hasDefaultProduct,
            defaultProductHasPrice,
            defaultProductInStock,
            productCardStatus,
            imagesCount);

        //Act
        var result = ProductCardPublishSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}