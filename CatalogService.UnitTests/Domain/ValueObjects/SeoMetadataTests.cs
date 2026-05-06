using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.ValueObjects;

public sealed class SeoMetadataTests
{
    [Fact]
    public void Create_Should_Succeed_With_Valid_Values()
    {
        // Arrange
        var title = SeoTitle.Create("Title").Value;
        var description = SeoDescription.Create("Description").Value;
        var keywords = SeoKeywords.Create("seo, marketing").Value;

        // Act
        var result = SeoMetadata.Create(title, description, keywords);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Assign_Title_Correctly()
    {
        //Arrange
        var title = SeoTitle.Create("My Title").Value;
        var description = SeoDescription.Create("Desc").Value;
        var keywords = SeoKeywords.Create("seo").Value;

        //Act
        var result = SeoMetadata.Create(title, description, keywords).Value;

        //Assert
        result.Title.Should().Be(title);
    }

    [Fact]
    public void Create_Should_Assign_Description_Correctly()
    {
        //Arrange
        var title = SeoTitle.Create("My Title").Value;
        var description = SeoDescription.Create("Desc").Value;
        var keywords = SeoKeywords.Create("seo").Value;

        //Act
        var result = SeoMetadata.Create(title, description, keywords).Value;

        //Assert
        result.Description.Should().Be(description);
    }

    [Fact]
    public void Create_Should_Assign_Keywords_Correctly()
    {
        //Arrange
        var title = SeoTitle.Create("My Title").Value;
        var description = SeoDescription.Create("Desc").Value;
        var keywords = SeoKeywords.Create("seo").Value;

        //Act
        var result = SeoMetadata.Create(title, description, keywords).Value;

        //Assert
        result.Keywords.Should().Be(keywords);
    }

    [Fact]
    public void SeoMetadata_Should_Be_Equal_When_Same_Values()
    {
        //Arrange
        var title = SeoTitle.Create("Title").Value;
        var description = SeoDescription.Create("Desc").Value;
        var keywords = SeoKeywords.Create("seo").Value;

        //Act
        var a = SeoMetadata.Create(title, description, keywords).Value;
        var b = SeoMetadata.Create(title, description, keywords).Value;

        //Assert
        a.Should().Be(b);
    }
}
