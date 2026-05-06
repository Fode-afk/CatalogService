using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class SlugTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Null_Or_Whitespace(string input)
    {
        //Act
        var result = Slug.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Too_Short()
    {
        //Arrange
        var input = "ab";

        //Act
        var result = Slug.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Too_Long()
    {
        //Arrange
        var input = new string('a', Slug.MaxLength + 1);

        //Act
        var result = Slug.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abc123")]
    [InlineData("abc-123")]
    [InlineData("abc-123-def")]
    public void Create_Should_Succeed_For_Valid_Slug(string input)
    {
        //Act
        var result = Slug.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(input.ToLower());
    }

    [Fact]
    public void Create_Should_Trim_And_Lowercase()
    {
        //Arrange
        var input = "   ABC-123   ";

        //Act
        var result = Slug.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("abc-123");
    }

    [Theory]
    [InlineData("-abc")]
    [InlineData("abc-")]
    [InlineData("abc--def")]
    [InlineData("abc_def")]
    [InlineData("abc def")]
    [InlineData("abc@123")]
    public void Create_Should_Fail_For_Invalid_Format(string input)
    {
        //Act
        var result = Slug.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Slug_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = Slug.Create("abc-123").Value;
        var b = Slug.Create("abc-123").Value;

        //Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Slug_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = Slug.Create("abc-123").Value;
        var b = Slug.Create("abc-456").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        //Arrange
        var slug = Slug.Create("abc-123").Value;

        //Act
        var result = slug.ToString();

        //Assert
        result.Should().Be("abc-123");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        //Arrange
        var slug = Slug.Create("abc-123").Value;

        //Act
        string result = slug;

        //Assert
        result.Should().Be("abc-123");
    }
}
