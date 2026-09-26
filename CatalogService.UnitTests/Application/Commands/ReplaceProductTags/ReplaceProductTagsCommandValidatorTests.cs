using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.Domain.Errors;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.ReplaceProductTags;

public class ReplaceProductTagsCommandValidatorTests
{
    private readonly ReplaceProductTagsCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(ReplaceProductTagsCommand.ProductId))]
    [InlineData(nameof(ReplaceProductTagsCommand.VendorId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand();
        command = fieldName switch
        {
            nameof(ReplaceProductTagsCommand.ProductId) => command with { ProductId = Guid.Empty },
            nameof(ReplaceProductTagsCommand.VendorId) => command with { VendorId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }

    [Fact]
    public void Should_Fail_When_Tags_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand()
            with
        { Tags = [] };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorCode(ProductErrorCodes.TagsRequired);
    }

    [Fact]
    public void Should_Fail_When_Tags_Exceeds_MaxCount()
    {
        // Arrange
        var tags = Enumerable.Range(0, 51).Select(i => $"tag{i}").ToList();
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand()
            with
        { Tags = tags };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorCode(ProductErrorCodes.MaxTagsReached);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Tags_Has_Exactly_MaxAllowedCount()
    {
        // Arrange
        var tags = Enumerable.Range(0, 50).Select(i => $"tag{i}").ToList();
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand()
            with
        { Tags = tags };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Should_Fail_When_Tags_Is_Null()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand()
            with
        { Tags = null! };

        // Act
        var act = () => _validator.TestValidate(command);

        // Assert
        act.Should().NotThrow();
    }
}
