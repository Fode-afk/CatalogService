using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Domain.Errors;
using CatalogService.Domain.ValueObjects;
using CatalogService.UnitTests.Fixtures;
using FluentValidation.TestHelper;

namespace CatalogService.UnitTests.Application.Commands.CreateProduct;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(nameof(CreateProductCommand.VendorId))]
    [InlineData(nameof(CreateProductCommand.CategoryId))]
    [InlineData(nameof(CreateProductCommand.BrandId))]
    public void Should_Fail_When_Id_Field_Is_Empty(string fieldName)
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand();
        command = fieldName switch
        {
            nameof(CreateProductCommand.VendorId) => command with { VendorId = Guid.Empty },
            nameof(CreateProductCommand.CategoryId) => command with { CategoryId = Guid.Empty },
            nameof(CreateProductCommand.BrandId) => command with { BrandId = Guid.Empty },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldName))
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(fieldName)
            .WithErrorCode(ProductErrorCodes.InvalidId);
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { Name = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ProductNameErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_Name_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Name = new string('a', ProductName.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ProductNameErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_Slug_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { Slug = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Slug)
            .WithErrorCode(SlugErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_Slug_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Slug = new string('a', Slug.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Slug)
            .WithErrorCode(SlugErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_Description_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { Description = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorCode(DescriptionErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Description = new string('a', Description.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorCode(DescriptionErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_ShortDescription_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { ShortDescription = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ShortDescription)
            .WithErrorCode(ShortDescriptionErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_ShortDescription_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { ShortDescription = new string('a', ShortDescription.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ShortDescription)
            .WithErrorCode(ShortDescriptionErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_SeoTitle_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { SeoTitle = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SeoTitle)
            .WithErrorCode(SeoTitleErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_SeoTitle_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoTitle = new string('a', SeoTitle.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SeoTitle)
            .WithErrorCode(SeoTitleErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_SeoDescription_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { SeoDescription = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SeoDescription)
            .WithErrorCode(SeoDescriptionErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_SeoDescription_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoDescription = new string('a', SeoDescription.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SeoDescription)
            .WithErrorCode(SeoDescriptionErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_SeoKeywords_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand() with { SeoKeywords = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SeoKeywords)
            .WithErrorCode(SeoKeywordsErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Should_Fail_When_SeoKeywords_Exceeds_MaxLength()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoKeywords = new string('a', SeoKeywords.MaxLength + 1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SeoKeywords)
            .WithErrorCode(SeoKeywordsErrorCodes.TooLong);
    }

    [Fact]
    public void Should_Fail_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Name = string.Empty, Slug = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}