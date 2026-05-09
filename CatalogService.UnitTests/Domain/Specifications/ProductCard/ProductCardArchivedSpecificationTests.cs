using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Product;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.ProductCard;

public sealed class ProductCardArchivedSpecificationTests
{
    [Theory]
    [InlineData(false, VendorSnapshotErrorCodes.CannotModify)]
    [InlineData(true, null)]
    public void Spec_Should_Return_Correct_Error(
        bool vendorActive,
        string? expectedErrorCode)
    {
        //Arrange
        var ctx = new ProductCardArchivedContext(vendorActive);

        //Act
        var result = ProductArchiveSpecification.Spec.IsSatisfiedBy(ctx);

        //Assert
        result.IsFailure.Should().Be(expectedErrorCode is not null);

        if (expectedErrorCode is not null)
            result.Error.Code.Should().Be(expectedErrorCode);
    }
}
