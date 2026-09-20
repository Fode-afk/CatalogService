using CatalogService.Application.Features.Commands.PublishProduct;
using CatalogService.Domain.Errors;
using CatalogService.UnitTests.Fixtures;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.PublishProduct;

public class PublishProductCommandValidatorTests
{
    private readonly PublishProductCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidPublishCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(PublishProductCommand.ProductId))]
    [InlineData(nameof(PublishProductCommand.VendorId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidPublishCommand();
        command = fieldName switch
        {
            nameof(PublishProductCommand.ProductId) => command with { ProductId = Guid.Empty },
            nameof(PublishProductCommand.VendorId) => command with { VendorId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }
}