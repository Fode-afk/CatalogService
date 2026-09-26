using CatalogService.Domain.Contexts;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Models;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Models = CatalogService.Domain.Models;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductReplaceAttributesTests
{
    private static readonly DateTimeOffset Now = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ReplaceAttributes_Should_Add_Attributes_And_Raise_Event_When_Valid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var attributes = new List<ProductAttribute> { ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true) };

        // Act
        var result = product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext(attributes), Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Attributes.Should().BeEquivalentTo(attributes);
        product.UpdatedAt.Should().Be(Now);
        product.Version.Should().Be(1);

        product.DomainEvents.Should().ContainSingle();
        var domainEvent = product.DomainEvents.Single().Should()
            .BeOfType<ProductAttributesReplacedDomainEvent>().Subject;
        domainEvent.ProductId.Should().Be(product.Id);
        domainEvent.Version.Should().Be(product.Version);
    }

    [Fact]
    public void ReplaceAttributes_Should_Remove_NonVariable_But_Keep_Variable_Attributes()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var variableAttribute = ProductDataTestFactory.CreateVariableAttribute(Guid.NewGuid());
        var firstNonVariable = ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true);

        product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([variableAttribute, firstNonVariable]), Now);
        product.ClearDomainEvents();

        var secondNonVariable = ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true);

        // Act
        var result = product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([secondNonVariable]), Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Attributes.Should().HaveCount(2);
        product.Attributes.Should().Contain(variableAttribute);
        product.Attributes.Should().Contain(secondNonVariable);
        product.Attributes.Should().NotContain(firstNonVariable);
    }

    [Fact]
    public void ReplaceAttributes_Should_Not_Add_When_CharacteristicId_Already_Exists_As_Variable()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var sharedCharacteristicId = Guid.NewGuid();
        var variableAttribute = ProductDataTestFactory.CreateVariableAttribute(sharedCharacteristicId);
        var unifyingSeed = ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true);

        product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([variableAttribute, unifyingSeed]), Now);
        product.ClearDomainEvents();

        var duplicateAttempt = ProductDataTestFactory.CreateAttribute(sharedCharacteristicId, isUnifying: true);

        // Act
        var result = product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([duplicateAttempt]), Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Attributes.Should().ContainSingle();
        product.Attributes.Single().Should().Be(variableAttribute);
        product.Attributes.Should().NotContain(duplicateAttempt);
    }

    [Fact]
    public void ReplaceAttributes_Should_Fail_When_ContextIsInvalid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var attributes = new List<ProductAttribute> { ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true) };
        var ctx = new ProductAttributesReplaceContext(VendorIsActive: false, CanBeModified: true, Attributes: attributes);

        // Act
        var result = product.ReplaceAttributes(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();
        product.Attributes.Should().BeEmpty();
        product.Version.Should().Be(0);
    }

    [Fact]
    public void ReplaceAttributes_Should_Fail_When_Attributes_Are_Empty()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        // Act
        var result = product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([]), Now);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReplaceAttributes_Should_Fail_When_Exceeding_MaxAttributes()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var attributes = Enumerable.Range(0, Models.Product.MaxAttributes + 1)
            .Select((_, i) => ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: i == 0))
            .ToList();

        // Act
        var result = product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext(attributes), Now);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public void ReplaceAttributes_Should_Fail_When_Unifying_Count_Is_Not_Exactly_One(int unifyingCount)
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var attributes = new List<ProductAttribute>
        {
            ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: unifyingCount >= 1),
            ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: unifyingCount >= 2)
        };

        // Act
        var result = product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext(attributes), Now);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ReplaceAttributes_ShouldValidateContext_WhenAttributesAreEqual()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var attributes = new List<ProductAttribute> { ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true) };
        var ctx = new ProductAttributesReplaceContext(VendorIsActive: false, CanBeModified: true, Attributes: attributes);
        product.ReplaceAttributes(ctx, Now);

        // Act
        var result = product.ReplaceAttributes(ctx, Now);

        // Assert
        result.IsFailure.Should().BeTrue();
        product.Attributes.Should().BeEmpty();
        product.Version.Should().Be(0);
    }
}