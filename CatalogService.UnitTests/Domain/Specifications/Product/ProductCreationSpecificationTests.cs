using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Product;

public sealed class ProductCreationSpecificationTests
{
    public static TheoryData<bool, bool, bool, string?> TestCases =>
       new()
       {
            { false, true, true, VendorSnapshotErrorCodes.CannotModify },
            { true, false, true, CategorySnapshotErrorCodes.Inactive },
            { true, true, false, BrandSnapshotErrorCodes.Inactive },
            { false, false, false, VendorSnapshotErrorCodes.CannotModify },
            { true, true, true, null }
       };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Spec_Should_Return_Correct_Error(
        bool vendorIsActive,
        bool categoryIsActive,
        bool brandIsAssignable,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCreationContext(
            vendorIsActive,
            categoryIsActive,
            brandIsAssignable);

        //Act
        var result = ProductCreationSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
