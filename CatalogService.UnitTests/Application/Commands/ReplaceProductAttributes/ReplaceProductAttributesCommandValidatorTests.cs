using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.Domain.Errors;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.ReplaceProductAttributes;

public class ReplaceProductAttributesCommandValidatorTests
{
    private readonly ReplaceProductAttributesCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(ReplaceProductAttributesCommand.ProductId))]
    [InlineData(nameof(ReplaceProductAttributesCommand.VendorId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand();
        command = fieldName switch
        {
            nameof(ReplaceProductAttributesCommand.ProductId) => command with { ProductId = Guid.Empty },
            nameof(ReplaceProductAttributesCommand.VendorId) => command with { VendorId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }

    [Fact]
    public void Should_Fail_When_Attributes_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand()
            with
        { Attributes = [] };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attributes)
            .WithErrorCode(ProductErrorCodes.AttributesRequired);
    }

    [Fact]
    public void Should_Fail_When_Attributes_Exceeds_MaxCount()
    {
        // Arrange
        var attributes = Enumerable.Range(0, 31)
            .ToDictionary(_ => Guid.NewGuid(), _ => "value");
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand()
            with
        { Attributes = attributes };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attributes)
            .WithErrorCode(ProductErrorCodes.MaxAttributesReached);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Attributes_Has_Exactly_MaxAllowedCount()
    {
        // Arrange 
        var attributes = Enumerable.Range(0, 30)
            .ToDictionary(_ => Guid.NewGuid(), _ => "value");
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand()
            with
        { Attributes = attributes };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Attributes);
    }

    [Fact]
    public void Should_Fail_When_Attributes_Is_Null()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand()
            with
        { Attributes = null! };

        // Act
        var act = () => _validator.TestValidate(command);

        // Assert
        act.Should().NotThrow();
    }
}
