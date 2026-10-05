using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductSuspendSpecificationTests
{
    public static TheoryData<bool, string?> TestCases =>
        new()
        {
            { false, ProductErrorCodes.CannotEditContent },
            { true, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool canBeModified,
        string? expectedErrorCode)
    {
        // Arrange
        var ctx = new ProductSuspendContext(canBeModified);

        // Act
        var result = ProductSuspendSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}