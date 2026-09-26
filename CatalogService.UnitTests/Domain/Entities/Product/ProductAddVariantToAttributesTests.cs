using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.UnitTests.Domain.Entities.Product;

public sealed class ProductAddVariantToAttributesTests
{
    private static readonly DateTimeOffset Now =
        new(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void AddVariantToAttributes_ShouldSucceed_WhenContextIsValid()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();
        var ctx = ProductContextsTestFactory.ValidAddVariantToAttributesContext();

        var characteristicId = Guid.NewGuid();

        var variantValues = new List<(
            Guid CharacteristicId,
            AttributeName Name,
            AttributeCharType CharType,
            AttributeGroupName? GroupName,
            AttributeVariableValue VariableValue)>
        {
            ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "Red")
        };

        // Act
        var result = product.AddVariantToAttributes(
            ctx,
            variantValues,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AddVariantToAttributes_ShouldCreateAttribute_WhenCharacteristicDoesNotExist()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var characteristicId = Guid.NewGuid();

        var variant = ProductDataTestFactory.CreateVariantValue(
            characteristicId: characteristicId,
            name: "Color",
            value: "Red");

        // Act
        var result = product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [variant],
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var attribute = product.Attributes
            .Single(a => a.CharacteristicId == characteristicId);

        attribute.IsVariable.Should().BeTrue();

        attribute.Name.Should().Be(variant.Name);
        attribute.CharType.Should().Be(variant.CharType);
        attribute.GroupName.Should().Be(variant.GroupName);
    }

    [Fact]
    public void AddVariantToAttributes_ShouldAddVariableValue_ToNewAttribute()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var characteristicId = Guid.NewGuid();

        var variant = ProductDataTestFactory.CreateVariantValue(
            characteristicId: characteristicId,
            value: "Red");

        // Act
        var result = product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [variant],
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var attribute = product.Attributes
            .Single(a => a.CharacteristicId == characteristicId);

        attribute.VariableValues
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(variant.VariableValue);
    }

    [Fact]
    public void AddVariantToAttributes_ShouldMarkExistingAttributeAsVariable()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var characteristicId = Guid.NewGuid();

        var attribute = ProductAttribute.Create(
            characteristicId,
            AttributeName.Create("Color").Value,
            AttributeValue.Create("Red").Value,
            AttributeCharType.Text).Value;

        product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([attribute]), Now);

        attribute.IsVariable.Should().BeFalse();

        var variant = ProductDataTestFactory.CreateVariantValue(
            characteristicId: characteristicId,
            value: "Blue",
            valueId: Guid.NewGuid());

        // Act
        var result = product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [variant],
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Attributes.First(a => a.CharacteristicId == characteristicId).IsVariable.Should().BeTrue();

        product.Attributes.First(a => a.CharacteristicId == characteristicId).VariableValues
            .Should()
            .Contain(variant.VariableValue);
    }

    [Fact]
    public void AddVariantToAttributes_ShouldAddMultipleValues_ToSameAttribute()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var characteristicId = Guid.NewGuid();

        var attribute = ProductAttribute.Create(
            characteristicId,
            AttributeName.Create("Color").Value,
            AttributeValue.Create("Red").Value,
            AttributeCharType.Text,
            isUnifying: false).Value;

        product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext([attribute]), Now);

        var variants = new List<(
            Guid CharacteristicId,
            AttributeName Name,
            AttributeCharType CharType,
            AttributeGroupName? GroupName,
            AttributeVariableValue VariableValue)>
        {
            ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "Red",
                valueId: Guid.NewGuid()),

            ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "Blue",
                valueId: Guid.NewGuid()),

            ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "Green",
                valueId: Guid.NewGuid())
        };

        // Act
        var result = product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            variants,
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var attributes = product.Attributes
            .Where(a => a.CharacteristicId == characteristicId)
            .ToList();

        attributes.Should().ContainSingle();

        attributes[0].VariableValues
            .Should()
            .HaveCount(3);
    }

    [Fact]
    public void AddVariantToAttributes_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var before = product.UpdatedAt;

        var variant = ProductDataTestFactory.CreateVariantValue();

        // Act
        var result = product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [variant],
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.UpdatedAt.Should().Be(Now);
        product.UpdatedAt.Should().NotBe(before);
    }

    [Fact]
    public void AddVariantToAttributes_ShouldIncreaseVersion()
    {
        // Arrange
        var product = ProductTestFactory.CreateValid();

        var versionBefore = product.Version;

        var variant = ProductDataTestFactory.CreateVariantValue();

        // Act
        var result = product.AddVariantToAttributes(
            ProductContextsTestFactory.ValidAddVariantToAttributesContext(),
            [variant],
            Now);

        // Assert
        result.IsSuccess.Should().BeTrue();

        product.Version.Should().Be(versionBefore + 1);
    }
}