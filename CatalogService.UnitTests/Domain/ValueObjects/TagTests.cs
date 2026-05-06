using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class TagTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Value_Is_Null_Or_Empty(string input)
    {
        //Act
        var result = Tag.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Value_Too_Long()
    {
        //Arrange
        var input = new string('a', Tag.MaxLength + 1);

        //Act
        var result = Tag.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Succeed_When_Value_Is_Valid()
    {
        //Arrange
        var input = "valid tag";

        //Act
        var result = Tag.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(input);
    }

    [Fact]
    public void Create_Should_Trim_Value()
    {
        //Arrange
        var input = "  trimmed text  ";

        //Act
        var result = Tag.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("trimmed text");
    }

    [Fact]
    public void Tag_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = Tag.Create("text").Value;
        var b = Tag.Create("text").Value;

        // Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Tag_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = Tag.Create("text1").Value;
        var b = Tag.Create("text2").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        var tag = Tag.Create("text").Value;

        // Act
        var result = tag.ToString();

        // Assert
        result.Should().Be("text");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var tag = Tag.Create("text").Value;

        // Act
        string result = tag;

        // Assert
        result.Should().Be("text");
    }
}
