using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Specifications.Common;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class TagsLimitSpecTests
{
    private sealed record TestContext(IReadOnlyCollection<Tag> Tags) : ITagsContext;

    private readonly TagsLimitSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Pass_When_Tags_Less_Than_Limit()
    {
        // Arrange
        var ctx = new TestContext([.. Enumerable.Range(1, 3).Select(i => Tag.Create(i.ToString()).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Tags_Equal_To_Max()
    {
        // Arrange
        var max = Models.Product.MaxTags;
        var ctx = new TestContext([.. Enumerable.Range(1, max).Select(i => Tag.Create(i.ToString()).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Tags_Exceed_Max()
    {
        // Arrange
        var max = Models.Product.MaxTags;
        var ctx = new TestContext([.. Enumerable.Range(1, max + 1).Select(i => Tag.Create(i.ToString()).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_MaxTagsReached_Error()
    {
        // Arrange
        var max = Models.Product.MaxTags;
        var ctx = new TestContext([.. Enumerable.Range(1, max + 1).Select(i => Tag.Create(i.ToString()).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.Error.Should().Be(ProductErrors.MaxTagsReached());
    }
}
