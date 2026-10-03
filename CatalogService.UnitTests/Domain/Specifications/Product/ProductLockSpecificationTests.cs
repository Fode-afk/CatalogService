using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductLockSpecificationTests
{
    public static TheoryData<bool, ProductStatus, string?> TestCases =>
        new()
        {
            { false, ProductStatus.Published, ProductErrorCodes.CannotModify },
            { true, ProductStatus.Draft, ProductErrorCodes.CannotModify },
            { false, ProductStatus.Draft, ProductErrorCodes.CannotModify },
            { true, ProductStatus.Published, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool canBeModified,
        ProductStatus productStatus,
        string? expectedErrorCode)
    {
        // Arrange
        var ctx = new ProductBlockContext(
            canBeModified,
            productStatus);

        // Act
        var result = ProductBlockSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}