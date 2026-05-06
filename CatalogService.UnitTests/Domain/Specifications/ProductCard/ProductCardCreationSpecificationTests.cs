using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.ProductCard;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardCreationSpecificationTests
{
    public static TheoryData<bool, bool, int, int, string?> TestCases =>
       new()
       {
            { false, true, 1, 1, VendorSnapshotErrorCodes.CannotModify },
            { true, false, 1, 1, CategorySnapshotErrorCodes.Inactive },
            { true, true, 0, 1, ProductCardErrorCodes.AttributesRequired },
            { true, true, 1, 0, ProductCardErrorCodes.TagsRequired },
            { true, true, Models.ProductCard.MaxAttributes + 1, 1, ProductCardErrorCodes.MaxAttributesReached },
            { true, true, 1, Models.ProductCard.MaxTags + 1, ProductCardErrorCodes.MaxTagsReached },
            { false, false, 0, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, true, Models.ProductCard.MaxAttributes, 1, null },
            { true, true, 1, Models.ProductCard.MaxTags, null },
            { true, true, 1, 1, null }
       };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        bool categoryIsActive,
        int attributesCount,
        int tagsCount,
        string? expectedErrorCode)
    {
        //Arrange
        var attributes = GenerateAttributes(attributesCount);
        var tags = GenerateTags(tagsCount);
        var ctx = new ProductCardCreationContext(
            vendorActive,
            categoryIsActive,
            attributes,
            tags);

        //Act
        var result = ProductCardCreationSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }

    private static List<ProductCardAttribute> GenerateAttributes(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => ProductCardAttribute.Create(i.ToString(), i.ToString()).Value)];

    private static List<Tag> GenerateTags(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => Tag.Create(i.ToString()).Value)];
}
