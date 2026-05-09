using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using FluentAssertions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class StatusIsNotArchivedSpecTests
{
    private sealed record TestContext(ProductCardStatus ProductCardStatus) : IProductContext;

    private readonly CanBeModifiedSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Fail_When_Status_Is_Archived()
    {
        // Arrange
        var ctx = new TestContext(ProductCardStatus.Archived);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(ProductCardStatus.Draft)]
    [InlineData(ProductCardStatus.Published)]
    public void Should_Pass_When_Status_Is_Not_Archived(ProductCardStatus status)
    {
        // Arrange
        var ctx = new TestContext(status);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_CannotModify_Error_When_Archived()
    {
        // Arrange
        var ctx = new TestContext(ProductCardStatus.Archived);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.Error.Should().Be(ProductErrors.CannotModify());
    }
}
