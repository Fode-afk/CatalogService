using CatalogService.Application.Features.Commands.ArchiveProduct;
using CatalogService.Domain.Errors;
using CatalogService.UnitTests.Fixtures;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.ArchiveProduct;

public class ArchiveProductCommandValidatorTests
{
    private readonly ArchiveProductCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidArchiveCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(ArchiveProductCommand.ProductId))]
    [InlineData(nameof(ArchiveProductCommand.VendorId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidArchiveCommand();
        command = fieldName switch
        {
            nameof(ArchiveProductCommand.ProductId) => command with { ProductId = Guid.Empty },
            nameof(ArchiveProductCommand.VendorId) => command with { VendorId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }
}