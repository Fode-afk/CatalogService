using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;
using migApp.Shared.Domain.Errors;

namespace CatalogService.UnitTests.Application.Commands.ReplaceProductTags;

public class ProductTagsReplacedDataBuilderTests
{
    [Fact]
    public void Build_Should_Return_Success_When_All_Tags_Are_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(
            tags: ["red", "sale"]);

        // Act
        var result = ProductTagsReplacedDataBuilder.Build(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(t => t.Value).Should().BeEquivalentTo("red", "sale");
    }

    [Fact]
    public void Build_Should_Return_Empty_Collection_When_Tags_List_Is_Empty()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(tags: []);

        // Act
        var result = ProductTagsReplacedDataBuilder.Build(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void Build_Should_Fail_When_A_Tag_Is_Invalid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(
            tags: [string.Empty]);

        // Act
        var result = ProductTagsReplacedDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(CommonErrorCodes.ValidationFailed);
    }

    [Fact]
    public void Build_Should_Accumulate_Errors_For_Multiple_Invalid_Tags()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(
            tags: [string.Empty, string.Empty]);

        // Act
        var result = ProductTagsReplacedDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().HaveCount(2);
    }

    [Fact]
    public void Build_Should_Fail_When_Some_Tags_Are_Invalid_And_Some_Valid()
    {
        // Arrange
        var command = ProductCommandTestsFactory.ValidReplaceTagsCommand(
            tags: ["red", string.Empty]);

        // Act
        var result = ProductTagsReplacedDataBuilder.Build(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().HaveCount(1);
    }
}
