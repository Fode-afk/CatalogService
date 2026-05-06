using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Specifications.Common;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class TagsRequiredSpecTests
{
    private sealed record TestContext(IReadOnlyCollection<Tag> Tags) : ITagsContext;

    private readonly TagsRequiredSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Fail_When_Tags_Empty()
    {
        // Arrange
        var ctx = new TestContext([]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_One_Tag_Exists()
    {
        // Arrange
        var ctx = new TestContext([Tag.Create("tag").Value]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Multiple_Tags_Exist()
    {
        // Arrange
        var ctx = new TestContext([.. Enumerable.Range(1, 3).Select(i => Tag.Create(i.ToString()).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
