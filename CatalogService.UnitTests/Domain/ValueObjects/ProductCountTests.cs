using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class ProductCountTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_Should_Fail_When_Value_Less_Than_Min(int input)
    {
        //Act
        var result = ProductCount.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(21)]
    [InlineData(100)]
    public void Create_Should_Fail_When_Value_Greater_Than_Max(int input)
    {
        //Act
        var result = ProductCount.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(20)]
    public void Create_Should_Succeed_For_Valid_Range(int input)
    {
        //Act
        var result = ProductCount.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(input);
    }

    [Fact]
    public void Create_Should_Allow_Min_Value()
    {
        //Act
        var result = ProductCount.Create(ProductCount.MinValue);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(0);
    }

    [Fact]
    public void Create_Should_Allow_Max_Value()
    {
        //Act
        var result = ProductCount.Create(ProductCount.MaxValue);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(20);
    }

    [Fact]
    public void IsEmpty_Should_Be_True_When_Value_Is_Zero()
    {
        //Arrange
        var count = ProductCount.Create(0).Value;

        //Act
        var result = count.IsEmpty;
        
        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_Should_Be_False_When_Value_Greater_Than_Zero()
    {
        //Arrange
        var count = ProductCount.Create(5).Value;

        //Act
        var result = count.IsEmpty;

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Zero_Should_Return_Zero_Value()
    {
        //Arrange
        var zero = ProductCount.Zero;

        //Assert
        zero.Value.Should().Be(0);
        zero.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void ProductCount_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = ProductCount.Create(5).Value;
        var b = ProductCount.Create(5).Value;

        //Assert
        a.Should().Be(b);
    }

    [Fact]
    public void ProductCount_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = ProductCount.Create(5).Value;
        var b = ProductCount.Create(10).Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void Implicit_Conversion_To_Int_Should_Work()
    {
        //Arrange
        var count = ProductCount.Create(7).Value;

        //Act
        int result = count;

        //Assert
        result.Should().Be(7);
    }
}
