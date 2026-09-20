using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;
using CatalogService.Domain.Specifications.Common;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using migApp.Shared.Enums.Characteristics;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class AttributesLimitSpecTests
{
    private sealed record TestContext(IReadOnlyCollection<ProductAttribute> Attributes) : IAttributesContext;

    private readonly AttributesLimitSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Pass_When_Attributes_Less_Than_Limit()
    {
        // Arrange
        var ctx = new TestContext([.. Enumerable
            .Range(1, 5)
            .Select(i => ProductAttribute.Create(
                Guid.NewGuid(),
                AttributeName.Create("Name").Value,
                AttributeValue.Create("Value").Value,
                AttributeCharType.Numeric).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Attributes_Equals_Max()
    {
        // Arrange
        var max = Models.Product.MaxAttributes;

        var ctx = new TestContext([.. Enumerable
            .Range(1, max)
            .Select(i => ProductAttribute.Create(
                Guid.NewGuid(),
                AttributeName.Create("Name").Value,
                AttributeValue.Create("Value").Value,
                AttributeCharType.Numeric).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Attributes_Exceed_Max()
    {
        // Arrange
        var max = Models.Product.MaxAttributes;

        var ctx = new TestContext([.. Enumerable
            .Range(1, max + 1)
            .Select(i => ProductAttribute.Create(
                Guid.NewGuid(),
                AttributeName.Create("Name").Value,
                AttributeValue.Create("Value").Value,
                AttributeCharType.Numeric).Value)]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
