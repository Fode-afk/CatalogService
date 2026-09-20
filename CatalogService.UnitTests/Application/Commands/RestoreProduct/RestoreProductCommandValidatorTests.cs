using CatalogService.Application.Features.Commands.RestoreProduct;
using CatalogService.Domain.Errors;
using CatalogService.UnitTests.Fixtures;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.RestoreProduct;

public class RestoreProductCommandValidatorTests
{
    private readonly RestoreProductCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidRestoreCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(RestoreProductCommand.ProductId))]
    [InlineData(nameof(RestoreProductCommand.VendorId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidRestoreCommand();
        command = fieldName switch
        {
            nameof(RestoreProductCommand.ProductId) => command with { ProductId = Guid.Empty },
            nameof(RestoreProductCommand.VendorId) => command with { VendorId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }
}
