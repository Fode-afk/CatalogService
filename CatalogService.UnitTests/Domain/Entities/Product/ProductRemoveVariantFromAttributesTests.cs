using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductRemoveVariantFromAttributesTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void RemoveVariantFromAttributes_ShouldSucceed_WhenContextIsValid()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldRemoveVariableValue()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithVariableAttribute(variantId: Guid.NewGuid());

        var variantId = product.Attributes
            .SelectMany(a => a.VariableValues)
            .Single()
            .ValueId;

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId!.Value,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Attributes
            .SelectMany(a => a.VariableValues)
            .Should()
            .NotContain(v => v.ValueId == variantId);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldRemoveAttribute_WhenLastVariableValueIsRemoved()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithVariableAttribute(variantId: Guid.NewGuid());

        var attribute = product.Attributes
            .Single(a => a.IsVariable);

        var variantId = attribute.VariableValues
            .Single()
            .ValueId;

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId!.Value,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Attributes
            .Should()
            .NotContain(attribute);

        product.Attributes
            .Should()
            .NotContain(a => a.IsVariable);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldKeepAttribute_WhenOtherVariableValuesRemain()
    {
        // Arrange
        var firstVariantId = Guid.NewGuid();
        var secondVariantId = Guid.NewGuid();

        var product = ProductTestFactory.CreateWithVariableAttribute(
            firstVariantId,
            secondVariantId);

        var attribute = product.Attributes
            .Single(a => a.IsVariable);

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            firstVariantId,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Attributes
            .Should()
            .Contain(attribute);

        attribute.VariableValues
            .Should()
            .ContainSingle();

        attribute.VariableValues
            .Should()
            .Contain(v => v.ValueId == secondVariantId);

        attribute.VariableValues
            .Should()
            .NotContain(v => v.ValueId == firstVariantId);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldNotRemoveNonVariableAttributes()
    {
        // Arrange
        var product = ProductTestFactory.CreateWithAttributes();

        var nonVariableAttribute = product.Attributes
            .First(a => !a.IsVariable);

        var variantId = Guid.NewGuid();

        var attributeCountBefore = product.Attributes.Count;

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Attributes
            .Should()
            .Contain(nonVariableAttribute);

        product.Attributes
            .Should()
            .HaveCount(attributeCountBefore);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.UpdatedAt.Should().Be(Now);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldIncreaseVersion()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        var versionBefore = product.Version;

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Version.Should().Be(versionBefore + 1);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldRaiseDomainEvent()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        // Act
        var result = product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProductVariantRemovedDomainEvent>();
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldRaiseEventWithCurrentVersion()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        // Act
        product.RemoveVariantFromAttributes(
            ProductContextsTestFactory.ValidRemoveVariantFromAttributesContext(),
            variantId,
            Now);

        // Assert
        var domainEvent = product.DomainEvents
            .Single()
            .Should()
            .BeOfType<ProductVariantRemovedDomainEvent>()
            .Subject;

        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void RemoveVariantFromAttributes_ShouldFail_WhenContextIsInvalid()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        var product = ProductTestFactory.CreateWithVariableAttribute(variantId);

        var ctx = new ProductRemoveVariantFromAttributesContext(
            VendorIsActive: false,
            CanBeModified: true);

        var versionBefore = product.Version;
        var updatedAtBefore = product.UpdatedAt;
        var attributeCountBefore = product.Attributes.Count;

        // Act
        var result = product.RemoveVariantFromAttributes(
            ctx,
            variantId,
            Now);

        // Assert
        result.IsFailure.Should().BeTrue();

        product.Version.Should().Be(versionBefore);
        product.UpdatedAt.Should().Be(updatedAtBefore);
        product.Attributes.Should().HaveCount(attributeCountBefore);
        product.DomainEvents.Should().BeEmpty();
    }
}