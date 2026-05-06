using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class ImageUrlTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Value_Is_Null_Or_Whitespace(string input)
    {
        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("htp://wrong.com")]
    [InlineData("://missing.scheme.com")]
    public void Create_Should_Fail_When_Invalid_Url_Format(string input)
    {
        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Fail_When_Url_Is_Relative()
    {
        //Arrange
        var input = "/images/test.png";

        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("ftp://example.com/image.png")]
    [InlineData("file://local/image.png")]
    public void Create_Should_Fail_When_Scheme_Is_Not_Http_Or_Https(string input)
    {
        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("http://example.com/image.png")]
    [InlineData("https://example.com/image.png")]
    public void Create_Should_Succeed_For_Valid_Http_Urls(string input)
    {
        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(input);
    }

    [Fact]
    public void Create_Should_Handle_Uppercase_Scheme()
    {
        //Arrange
        var input = "HTTPS://example.com/image.png";

        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("https://example.com/image.png");
    }

    [Fact]
    public void Create_Should_Trim_Value()
    {
        //Arrange
        var input = "   https://example.com/image.png   ";

        //Act
        var result = ImageUrl.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("https://example.com/image.png");
    }

    [Fact]
    public void ImageUrl_Should_Be_Equal_When_Values_Are_Same()
    {
        //Arrange
        var a = ImageUrl.Create("https://example.com/a.png").Value;
        var b = ImageUrl.Create("https://example.com/a.png").Value;

        //Assert
        a.Should().Be(b);
    }

    [Fact]
    public void ImageUrl_Should_Not_Be_Equal_When_Values_Differ()
    {
        //Arrange
        var a = ImageUrl.Create("https://example.com/a.png").Value;
        var b = ImageUrl.Create("https://example.com/b.png").Value;

        //Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        //Arrange
        var url = ImageUrl.Create("https://example.com/a.png").Value;

        //Assert
        url.ToString().Should().Be("https://example.com/a.png");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        //Arrange
        var url = ImageUrl.Create("https://example.com/a.png").Value;

        //Act
        string result = url;

        //Assert
        result.Should().Be("https://example.com/a.png");
    }
}
