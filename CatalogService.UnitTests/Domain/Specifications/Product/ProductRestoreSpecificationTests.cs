using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductRestoreSpecificationTests
{
    public static TheoryData<bool, string?> TestCases =>
        new()
        {
            { false, VendorSnapshotErrorCodes.CannotModify },
            { true, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        string? expectedErrorCode)
    {
        // Arrange
        var ctx = new ProductRestoreContext(vendorIsActive);

        // Act
        var result = ProductRestoreSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}