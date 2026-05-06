using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class SeoKeywordsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Null_Or_Whitespace(string input)
    {
        //Act
        var result = SeoKeywords.Create(input);

        //Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Normalize_To_Lowercase_And_Trim()
    {
        //Arrange
        var input = "  SEO , MARKETING , dev  ";

        //Act
        var result = SeoKeywords.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("seo, marketing, dev");
    }

    [Fact]
    public void Create_Should_Remove_Duplicates()
    {
        //Arrange
        var input = "seo, SEO, marketing, MARKETING";

        //Act
        var result = SeoKeywords.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("seo, marketing");
    }

    [Fact]
    public void Create_Should_Remove_Empty_Entries()
    {
        //Arrange
        var input = "seo,, ,marketing,,,dev";

        //Act
        var result = SeoKeywords.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("seo, marketing, dev");
    }

    [Fact]
    public void Create_Should_Preserve_Order_Of_First_Occurrence()
    {
        //Arrange
        var input = "b, a, c, b, a";

        //Act
        var result = SeoKeywords.Create(input);
        
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("b, a, c");
    }

    [Fact]
    public void Create_Should_Fail_When_Normalized_Value_Too_Long()
    {
        // Arrange
        var input = string.Join(",",
            Enumerable.Range(1, 200)
                .Select(i => $"keyword{i}"));

        // Act
        var result = SeoKeywords.Create(input);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Succeed_When_Normalized_Length_Is_Valid()
    {
        //Arrange
        var input = "seo, marketing, dev";

        //Act
        var result = SeoKeywords.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("seo, marketing, dev");
    }

    [Fact]
    public void Single_Keyword_Should_Work()
    {
        //Arrange
        var input = "SEO";

        //Act
        var result = SeoKeywords.Create(input);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("seo");
    }

    [Fact]
    public void Equality_Should_Work_For_Same_Normalized_Value()
    {
        //Arrange
        var a = SeoKeywords.Create("SEO, marketing").Value;
        var b = SeoKeywords.Create("seo, MARKETING").Value;

        //Act
        a.Should().Be(b);
    }

    [Fact]
    public void Value_Should_Be_Consistent_After_Normalization()
    {
        //Arrange
        var input = "  SEO , seo , Seo  ";

        //Act
        var result = SeoKeywords.Create("  SEO , seo , Seo  ");

        //Assert
        result.Value.Value.Should().Be("seo");
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        var seoTitle = SeoKeywords.Create("text").Value;

        // Act
        var result = seoTitle.ToString();

        // Assert
        result.Should().Be("text");
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var seoTitle = SeoKeywords.Create("text").Value;

        // Act
        string result = seoTitle;

        // Assert
        result.Should().Be("text");
    }
}
