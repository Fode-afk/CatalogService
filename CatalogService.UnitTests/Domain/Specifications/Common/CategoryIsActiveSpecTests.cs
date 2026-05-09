using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class CategoryIsActiveSpecTests
{
    private sealed record TestContext(bool CategoryIsActive) : ICategoryContext;

    private readonly CategoryIsActiveSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Fail_When_Category_Is_Not_Active()
    {
        // Arrange
        var ctx = new TestContext(false);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Category_Is_Active()
    {
        // Arrange
        var ctx = new TestContext(true);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_Inactive_Error_When_Inactive()
    {
        // Arrange
        var ctx = new TestContext(false);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.Error.Should().Be(CategorySnapshotErrors.Inactive());
    }
}