using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class ProductCardAttributeTests
{
    [Fact]
    public void Create_Should_Fail_When_Name_Is_Invalid()
    {
        // Arrange
        var name = ""; // invalid
        var value = "some value";

        // Act
        var result = ProductCardAttribute.Create(name, value);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Value_Is_Invalid()
    {
        // Arrange
        var name = "Color";
        var value = ""; // invalid

        // Act
        var result = ProductCardAttribute.Create(name, value);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Succeed_When_Both_Valid()
    {
        // Arrange
        var name = "Color";
        var value = "Red";

        // Act
        var result = ProductCardAttribute.Create(name, value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().NotBeNull();
        result.Value.Value.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_Return_Proper_Name_And_Value()
    {
        // Arrange
        var name = "Color";
        var value = "Red";

        // Act
        var result = ProductCardAttribute.Create(name, value).Value;

        // Assert
        result.Name.Value.Should().Be(name);
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Attributes_Should_Be_Equal_When_Same_Values()
    {
        //Arrange
        var a = ProductCardAttribute.Create("Color", "Red").Value;
        var b = ProductCardAttribute.Create("Color", "Red").Value;

        //Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Attributes_Should_Not_Be_Equal_When_Different()
    {
        //Arrange
        var a = ProductCardAttribute.Create("Color", "Red").Value;
        var b = ProductCardAttribute.Create("Size", "Red").Value;

        //Assert
        a.Should().NotBe(b);
    }
}
