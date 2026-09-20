using CatalogService.Application.Features.Commands.UnpublishProduct;
using CatalogService.Domain.Errors;
using CatalogService.UnitTests.Fixtures;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.UnpublishProduct;

public class UnpublishProductCommandValidatorTests
{
    private readonly UnpublishProductCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidUnpublishCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(UnpublishProductCommand.ProductId))]
    [InlineData(nameof(UnpublishProductCommand.VendorId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidUnpublishCommand();
        command = fieldName switch
        {
            nameof(UnpublishProductCommand.ProductId) => command with { ProductId = Guid.Empty },
            nameof(UnpublishProductCommand.VendorId) => command with { VendorId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }
}