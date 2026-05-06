using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.ProductCard;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardAddImageSpecificationTests
{
    public static TheoryData<bool, ProductCardStatus, int, string?> TestCases =>
        new()
        {
            { false, ProductCardStatus.Draft, 1, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Archived, 1, ProductCardErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, Models.ProductCard.MaxImages, ProductCardImageErrorCodes.MaxImagesReached },
            { false, ProductCardStatus.Archived, Models.ProductCard.MaxImages, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, Models.ProductCard.MaxImages - 1, null },
            { true, ProductCardStatus.Published, 1, null },
            { true, ProductCardStatus.Draft, 1, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        ProductCardStatus status,
        int imagesCount,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardAddImageContext(
            vendorActive,
            status,
            imagesCount);

        //Act
        var result = ProductCardAddImageSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
