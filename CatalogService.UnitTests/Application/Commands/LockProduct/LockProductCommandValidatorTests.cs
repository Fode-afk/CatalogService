using CatalogService.Application.Features.Commands.LockProduct;
using CatalogService.Domain.Errors;
using CatalogService.TestCommon.Fixtures;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.LockProduct;

public class LockProductCommandValidatorTests
{
    private readonly LockProductCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidLockCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_ProductId_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidLockCommand() with { ProductId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductId)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }
}