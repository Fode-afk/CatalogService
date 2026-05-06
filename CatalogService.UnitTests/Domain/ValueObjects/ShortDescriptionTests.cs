using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class ShortDescriptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Value_Is_Null_Or_Empty(string input)
    {
        //Act
        var result = ShortDescription.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Value_Too_Long()
    {
        //Arrange
        var input = new string('a', ShortDescription.MaxLength + 1);

        //Act
        var result = ShortDescription.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Succeed_When_Value_Is_Valid()
    {
        //Arrange
        var input = "valid shortDescription";

        //Act
        var result = ShortDescription.Create(input);

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
        var result = ShortDescription.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("trimmed text");
    }

    [Fact]
    public void ShortDescription_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = ShortDescription.Create("text").Value;
        var b = ShortDescription.Create("text").Value;

        // Assert
        a.Should().Be(b);
    }

    [Fact]
    public void ShortDescription_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = ShortDescription.Create("text1").Value;
        var b = ShortDescription.Create("text2").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        var shortDescription = ShortDescription.Create("text").Value;

        // Act
        var result = shortDescription.ToString();

        // Assert
        result.Should().Be("text");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var shortDescription = ShortDescription.Create("text").Value;

        // Act
        string result = shortDescription;

        // Assert
        result.Should().Be("text");
    }
}
