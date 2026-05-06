using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.ProductCard;
using CatalogService.Domain.ValueObjects;
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
            { true, ProductCardStatus.Archived, 0, ProductCardErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, 0, ProductCardErrorCodes.AttributesRequired },
            { false, ProductCardStatus.Archived, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, Models.ProductCard.MaxAttributes + 1,  ProductCardErrorCodes.MaxAttributesReached },
            { true, ProductCardStatus.Draft, Models.ProductCard.MaxAttributes, null },
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
        var result = ProductCardAttributesReplacedSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }

    private static List<ProductCardAttribute> GenerateAttributes(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => ProductCardAttribute.Create(i.ToString(), i.ToString()).Value)];
}
