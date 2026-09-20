using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductUnpublishSpecificationTests
{
    public static TheoryData<bool, bool, string?> TestCases =>
        new()
        {
            { false, true, VendorSnapshotErrorCodes.CannotModify },
            { true, false, ProductErrorCodes.CannotModify },
            { false, false, VendorSnapshotErrorCodes.CannotModify },
            { true, true, null }
        };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        bool canBeModified,
        string? expectedErrorCode)
    {
        // Arrange
        var ctx = new ProductUnpublishContext(
            vendorIsActive,
            canBeModified);

        // Act
        var result = ProductUnpublishSpecification.Spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}