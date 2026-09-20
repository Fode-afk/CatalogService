using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;
using migApp.Shared.Enums.Products;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductUnlockSpecificationTests
{
    public static TheoryData<ProductStatus, string?> TestCases =>
        new()
        {
            { ProductStatus.Archived, ProductErrorCodes.CannotModify },
            { ProductStatus.Draft, null },
            { ProductStatus.Published, null },
            { ProductStatus.Suspended, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        ProductStatus productStatus,
        string? expectedErrorCode)
    {
        // Arrange
        var ctx = new ProductUnlockContext(productStatus);

        // Act
        var result = ProductUnlockSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}