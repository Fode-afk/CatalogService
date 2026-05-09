using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardAttributesReplacedSpecificationTests
{
    public static TheoryData<bool, ProductCardStatus, int, string?> TestCases =>
        new()
        {
            { false, ProductCardStatus.Draft, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Archived, 0, ProductErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, 0, ProductErrorCodes.AttributesRequired },
            { false, ProductCardStatus.Archived, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, Models.Product.MaxAttributes + 1,  ProductErrorCodes.MaxAttributesReached },
            { true, ProductCardStatus.Draft, Models.Product.MaxAttributes, null },
            { true, ProductCardStatus.Draft, 1, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        ProductCardStatus status,
        int attributesCount,
        string? expectedErrorCode)
    {
        //Arrange
        var attributes = GenerateAttributes(attributesCount);
        var ctx = new ProductCardAttributesReplacedContext(
            vendorActive,
            status,
            attributes);

        //Act
        var result = ProductAttributesReplaceSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }

    private static List<ProductAttribute> GenerateAttributes(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => ProductAttribute.Create(i.ToString(), i.ToString()).Value)];
}
