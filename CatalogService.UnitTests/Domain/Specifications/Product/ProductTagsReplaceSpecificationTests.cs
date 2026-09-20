using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductTagsReplaceSpecificationTests
{
    public static TheoryData<bool, bool, int, string?> TestCases =>
        new()
        {
            { false, true, 1, VendorSnapshotErrorCodes.CannotModify },
            { true, false, 1, ProductErrorCodes.CannotModify },
            { true, true, 0, ProductErrorCodes.TagsRequired },
            { true, true, CatalogService.Domain.Models.Product.MaxTags + 1, ProductErrorCodes.MaxTagsReached },
            { true, true, 1, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        bool canBeModified,
        int tagsCount,
        string? expectedErrorCode)
    {
        // Arrange
        var tags = Enumerable
            .Range(0, tagsCount)
            .Select(i => Tag.Create($"tag-{i}").Value)
            .ToList();

        var ctx = new ProductTagsReplaceContext(
            vendorIsActive,
            canBeModified,
            tags);

        // Act
        var result = ProductTagsReplaceSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}