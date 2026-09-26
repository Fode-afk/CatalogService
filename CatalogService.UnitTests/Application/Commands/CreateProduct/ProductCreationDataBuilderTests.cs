using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Domain.Errors;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Domain.Errors;

namespace CatalogService.UnitTests.Application.Commands.CreateProduct;

public class ProductCreationDataBuilderTests
{
    [Fact]
    public void Build_Should_Return_Success_When_Command_Is_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand();

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Value.Should().Be(command.Name);
        result.Value.Slug.Value.Should().Be(command.Slug);
        result.Value.Description.Value.Should().Be(command.Description);
        result.Value.ShortDescription.Value.Should().Be(command.ShortDescription);
        result.Value.SeoMetadata.Title.Value.Should().Be(command.SeoTitle);
        result.Value.SeoMetadata.Description.Value.Should().Be(command.SeoDescription);
        result.Value.SeoMetadata.Keywords.Value.Should().Be(command.SeoKeywords);
    }

    [Fact]
    public void Build_Should_Fail_When_Name_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Name = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(CommonErrorCodes.ValidationFailed);
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(ProductNameErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Fail_When_Slug_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Slug = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(SlugErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Fail_When_Description_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Description = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(DescriptionErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Fail_When_ShortDescription_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { ShortDescription = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(ShortDescriptionErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Fail_When_SeoTitle_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoTitle = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(SeoTitleErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Fail_When_SeoDescription_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoDescription = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(SeoDescriptionErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Fail_When_SeoKeywords_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoKeywords = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(SeoKeywordsErrorCodes.NullOrEmpty);
    }

    [Fact]
    public void Build_Should_Accumulate_All_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { Name = string.Empty, Slug = string.Empty, SeoTitle = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(ProductNameErrorCodes.NullOrEmpty);
        errorCodes.Should().Contain(SlugErrorCodes.NullOrEmpty);
        errorCodes.Should().Contain(SeoTitleErrorCodes.NullOrEmpty);
        errorCodes.Should().HaveCount(3);
    }

    [Fact]
    public void Build_Should_Not_Build_SeoMetadata_When_Any_Seo_Field_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidCreateCommand()
            with
        { SeoTitle = string.Empty };

        // Act
        var result = ProductCreationDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];

        errorCodes.Should().ContainSingle()
            .Which.Should().Be(SeoTitleErrorCodes.NullOrEmpty);
    }
}
