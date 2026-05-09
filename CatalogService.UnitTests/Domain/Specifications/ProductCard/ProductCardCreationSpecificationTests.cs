using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Specifications.Product;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardCreationSpecificationTests
{
    public static TheoryData<bool, bool, bool, int, int, string?> TestCases =>
       new()
       {
            { false, true, true, 1, 1, VendorSnapshotErrorCodes.CannotModify },
            { true, false, true, 1, 1, CategorySnapshotErrorCodes.Inactive },
            { true, true, false, 1, 1, BrandSnapshotErrorCodes.Inactive },
            { true, true, true, 0, 1, ProductErrorCodes.AttributesRequired },
            { true, true, true, 1, 0, ProductErrorCodes.TagsRequired },
            { true, true, true, Models.Product.MaxAttributes + 1, 1, ProductErrorCodes.MaxAttributesReached },
            { true, true, true, 1, Models.Product.MaxTags + 1, ProductErrorCodes.MaxTagsReached },
            { false, false, false, 0, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, true, true, Models.Product.MaxAttributes, 1, null },
            { true, true, true, 1, Models.Product.MaxTags, null },
            { true, true, true, 1, 1, null }
       };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        bool categoryIsActive,
        bool brandIsActive,
        int attributesCount,
        int tagsCount,
        string? expectedErrorCode)
    {
        //Arrange
        var attributes = GenerateAttributes(attributesCount);
        var tags = GenerateTags(tagsCount);
        var ctx = new ProductCreationContext(
            vendorIsActive,
            categoryIsActive,
            brandIsActive,
            attributes,
            tags);

        //Act
        var result = ProductCreationSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }

    private static List<ProductAttribute> GenerateAttributes(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => ProductAttribute.Create(i.ToString(), i.ToString()).Value)];

    private static List<Tag> GenerateTags(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => Tag.Create(i.ToString()).Value)];
}
