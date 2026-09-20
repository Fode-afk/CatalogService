using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class ProductNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Null_Or_Whitespace(string input)
    {
        //Act
        var result = ProductName.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Too_Short()
    {
        //Arrange
        var input = new string('a', ProductName.MinLength - 1);

        //Act
        var result = ProductName.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Too_Long()
    {
        //Arrange
        var input = new string('a', ProductName.MaxLength + 1);

        //Act
        var result = ProductName.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Trim_Value()
    {
        //Arrange
        var input = "   John   ";

        //Act
        var result = ProductName.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("John");
    }

    [Fact]
    public void Create_Should_Succeed_With_Valid_Length()
    {
        //Arrange
        var input = "John";

        //Act
        var result = ProductName.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("John");
    }

    [Fact]
    public void Name_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = ProductName.Create("John").Value;
        var b = ProductName.Create("John").Value;

        //Assert
        a.Should().Be(b);
    }

    [Fact]
    public void Name_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = ProductName.Create("John").Value;
        var b = ProductName.Create("Mike").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        var name = ProductName.Create("text").Value;

        // Act
        var result = name.ToString();

        // Assert
        result.Should().Be("text");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        //Arrange
        var name = ProductName.Create("John").Value;

        //Act
        string result = name;

        //Assert
        result.Should().Be("John");
    }

    [Fact]
    public void Normalize_Should_Lowercase_And_Remove_Spaces()
    {
        //Arrange
        var name = ProductName.Create("John Doe").Value;

        //Act
        var result = ProductName.Normalize(name);

        //Assert
        result.Should().Be("johndoe");
    }

    [Fact]
    public void Normalize_Should_Handle_Extra_Spaces()
    {
        //Arrange
        var name = ProductName.Create("   John   Doe   ").Value;

        //Act
        var result = ProductName.Normalize(name);

        //Assert
        result.Should().Be("johndoe");
    }

    [Fact]
    public void Normalize_Should_Work_For_Single_Word()
    {
        //Arrange
        var name = ProductName.Create("John").Value;

        //Act
        var result = ProductName.Normalize(name);

        //Assert
        result.Should().Be("john");
    }

    [Fact]
    public void Normalize_Should_Throw_When_Null()
    {
        //Arange
        Action act = () => ProductName.Normalize(null);

        //Act
        act.Should().Throw<NullReferenceException>();
    }
}
