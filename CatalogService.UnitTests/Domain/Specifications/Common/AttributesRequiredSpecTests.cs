using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;
using CatalogService.Domain.Specifications.Common;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.UnitTests.Domain.Specifications.Common;

public sealed class AttributesRequiredSpecTests
{
    private sealed record TestContext(IReadOnlyCollection<ProductAttribute> Attributes) : IAttributesContext;

    private readonly AttributesRequiredSpec<TestContext> _spec = new();

    [Fact]
    public void Should_Fail_When_Attributes_Empty()
    {
        // Arrange
        var ctx = new TestContext([]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Attributes_Exist()
    {
        // Arrange
        var ctx = new TestContext([ProductAttribute.Create(
            Guid.NewGuid(),
            AttributeName.Create("Name").Value,
            AttributeValue.Create("Value").Value,
            AttributeCharType.Numeric).Value]);

        // Act
        var result = _spec.IsSatisfiedBy(ctx);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Multiple_Attributes_Exist()
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
}
