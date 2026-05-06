using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.ProductCard;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardTagsReplacedSpecificationTests
{
    public static TheoryData<bool, ProductCardStatus, int, string?> TestCases =>
        new()
        {
            { false, ProductCardStatus.Draft, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Archived, 0, ProductCardErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, 0, ProductCardErrorCodes.TagsRequired },
            { false, ProductCardStatus.Archived, 0, VendorSnapshotErrorCodes.CannotModify },
            { true, ProductCardStatus.Draft, Models.ProductCard.MaxTags + 1,  ProductCardErrorCodes.MaxTagsReached },
            { true, ProductCardStatus.Draft, Models.ProductCard.MaxTags, null },
            { true, ProductCardStatus.Draft, 1, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        ProductCardStatus status,
        int tagsCount,
        string? expectedErrorCode)
    {
        //Arrange
        var tags = GenerateTags(tagsCount);
        var ctx = new ProductCardTagsReplacedContext(
            vendorActive,
            status,
            tags);

        //Act
        var result = ProductCardTagsReplacedSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }

    private static List<Tag> GenerateTags(int count) =>
        [.. Enumerable
            .Range(0, count)
            .Select(i => Tag.Create(i.ToString()).Value)];
}