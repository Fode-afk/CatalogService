using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class BrandTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Should_Fail_When_Value_Is_Null_Or_Empty(string input)
    {
        //Act
        var result = Brand.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Value_Too_Long()
    {
        //Arrange
        var input = new string('a', Brand.MaxLength + 1);

        //Act
        var result = Brand.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Succeed_When_Value_Is_Valid()
    {
        //Arrange
        var input = "valid brand";

        //Act
        var result = Brand.Create(input);

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
        var result = Brand.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("trimmed text");
    }

    [Fact]
    public void Create_Should_Normalize_Tabs_And_NewLines()
    {
        //Arrange
        var input = "Nike\t\tAir\nMax";

        //Act
        var result = Brand.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Nike Air Max");
    }

    [Fact]
    public void Create_Should_Normalize_Complex_Whitespace()
    {
        //Arrange
        var input = "   Nike   Air    Max   ";

        //Act
        var result = Brand.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Nike Air Max");
    }

    [Fact]
    public void Brand_Should_Be_Equal_When_Values_Are_Same_After_Normalization()
    {
        //Arrange
        var a = Brand.Create("Nike   Air").Value;
        var b = Brand.Create("Nike Air").Value;

        //Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Brand_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = Brand.Create("text").Value;
        var b = Brand.Create("text").Value;

        // Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Brand_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = Brand.Create("Nike").Value;
        var b = Brand.Create("Adidas").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        var brand = Brand.Create("text").Value;

        // Act
        var result = brand.ToString();

        // Assert
        result.Should().Be("text");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var brand = Brand.Create("text").Value;

        // Act
        string result = brand;

        // Assert
        result.Should().Be("text");
    }
}
