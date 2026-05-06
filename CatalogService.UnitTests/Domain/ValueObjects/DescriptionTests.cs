using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class DescriptionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Value_Is_Null_Or_Empty(string input)
    {
        //Act
        var result = Description.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Value_Too_Long()
    {
        //Arrange
        var input = new string('a', Description.MaxLength + 1);

        //Act
        var result = Description.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Succeed_When_Value_Is_Valid()
    {
        //Arrange
        var input = "valid description";

        //Act
        var result = Description.Create(input);

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
        var result = Description.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("trimmed text");
    }

    [Fact]
    public void Description_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = Description.Create("text").Value;
        var b = Description.Create("text").Value;

        // Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Description_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = Description.Create("text1").Value;
        var b = Description.Create("text2").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        var description = Description.Create("text").Value;

        // Act
        var result = description.ToString();

        // Assert
        result.Should().Be("text");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var description = Description.Create("text").Value;

        // Act
        string result = description;

        // Assert
        result.Should().Be("text");
    }
}
