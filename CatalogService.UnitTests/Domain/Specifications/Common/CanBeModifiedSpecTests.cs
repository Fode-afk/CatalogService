using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class CanBeModifiedSpecTests
{
    private sealed record TestContext(bool CanBeModified) : IProductContext;

    private readonly CanBeModifiedSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Fail_When_Can_Not_Be_Modified()
    {
        // Arrange
        var ctx = new TestContext(false);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Can_Be_Modified()
    {
        // Arrange
        var ctx = new TestContext(true);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_CannotModify_Error_When_Can_Not_Be_Modified()
    {
        // Arrange
        var ctx = new TestContext(false);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.Error.Should().Be(ProductErrors.CannotModify());
    }
}
