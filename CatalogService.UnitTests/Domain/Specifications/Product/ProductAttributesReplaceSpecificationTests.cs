using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Specifications.Product;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductAttributesReplaceSpecificationTests
{
    public static TheoryData<bool, bool, int, int, string?> TestCases =>
        new()
        {
            { false, true, 1, 1, VendorSnapshotErrorCodes.CannotModify },
            { true, false, 1, 1, ProductErrorCodes.CannotEditContent },
            { true, true, 0, 0, ProductErrorCodes.AttributesRequired },
            { true, true, CatalogService.Domain.Models.Product.MaxAttributes + 1, 0, ProductErrorCodes.MaxAttributesReached },
            { true, true, 2, 2, ProductAttributeErrorCodes.UnifyingAttributeRequired },
            { true, true, 2, 1, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        bool canBeModified,
        int attributesCount,
        int unifyingAttributesCount,
        string? expectedErrorCode)
    {
        // Arrange
        var attributes = Enumerable
            .Range(0, attributesCount)
            .Select(i => ProductAttribute.Create(
                Guid.NewGuid(),
                AttributeName.Create("Name").Value,
                AttributeValue.Create("Value").Value,
                AttributeCharType.Numeric).Value);

        var unifyingAttributes = Enumerable
            .Range(0, unifyingAttributesCount)
            .Select(i => ProductAttribute.Create(
                Guid.NewGuid(),
                AttributeName.Create("Name").Value,
                AttributeValue.Create("Value").Value,
                AttributeCharType.Numeric,
                isUnifying: true).Value);

        var ctx = new ProductAttributesReplaceContext(
            vendorIsActive,
            canBeModified,
            [.. attributes, .. unifyingAttributes]);

        // Act
        var result = ProductAttributesReplaceSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}