using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using FluentAssertions;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.UnitTests.Domain.Entities;

public sealed class ProductAttributeTests
{
    private static ProductAttribute CreateValid(bool variable = false)
    {
        var name = AttributeName.Create("Color").Value;

        if (variable)
            return ProductAttribute.CreateVariable(Guid.NewGuid(), name, AttributeCharType.Text).Value;

        var value = AttributeValue.Create("Red").Value;
        return ProductAttribute.Create(Guid.NewGuid(), name, value, AttributeCharType.Text).Value;
    }

    [Fact]
    public void Create_Should_Succeed_And_Set_Properties()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var name = AttributeName.Create("Color").Value;
        var value = AttributeValue.Create("Red").Value;

        // Act
        var result = ProductAttribute.Create(characteristicId, name, value, AttributeCharType.Text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CharacteristicId.Should().Be(characteristicId);
        result.Value.Name.Should().Be(name);
        result.Value.Value.Should().Be(value);
        result.Value.IsVariable.Should().BeFalse();
        result.Value.VariableValues.Should().BeEmpty();
    }

    [Fact]
    public void CreateVariable_Should_Succeed_With_Null_Value_And_IsVariable_True()
    {
        // Act
        var result = ProductAttribute.CreateVariable(Guid.NewGuid(), AttributeName.Create("Size").Value, AttributeCharType.Text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsVariable.Should().BeTrue();
        result.Value.Value.Should().BeNull();
    }

    [Fact]
    public void MarkAsVariable_Should_Set_IsVariable_True_And_Clear_Value()
    {
        // Arrange
        var attribute = CreateValid();

        // Act
        attribute.MarkAsVariable();

        // Assert
        attribute.IsVariable.Should().BeTrue();
        attribute.Value.Should().BeNull();
    }

    [Fact]
    public void AddVariableValue_Should_Fail_When_Attribute_Is_Not_Variable()
    {
        // Arrange
        var attribute = CreateValid(variable: false);
        var variableValue = new AttributeVariableValue("Red", Guid.NewGuid());

        // Act
        var result = attribute.AddVariableValue(variableValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        attribute.VariableValues.Should().BeEmpty();
    }

    [Fact]
    public void AddVariableValue_Should_Add_When_Attribute_Is_Variable()
    {
        // Arrange
        var attribute = CreateValid(variable: true);
        var variableValue = new AttributeVariableValue("Red", Guid.NewGuid());

        // Act
        var result = attribute.AddVariableValue(variableValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        attribute.VariableValues.Should().ContainSingle().Which.Should().Be(variableValue);
    }

    [Fact]
    public void AddVariableValue_Should_Be_Idempotent_When_ValueId_Already_Exists()
    {
        // Arrange
        var attribute = CreateValid(variable: true);
        var valueId = Guid.NewGuid();
        attribute.AddVariableValue(new AttributeVariableValue("Red", valueId));

        // Act
        var result = attribute.AddVariableValue(new AttributeVariableValue("Blue", valueId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        attribute.VariableValues.Should().ContainSingle();
    }

    [Fact]
    public void RemoveVariableValue_Should_Remove_Existing_Value()
    {
        // Arrange
        var attribute = CreateValid(variable: true);
        var valueId = Guid.NewGuid();
        attribute.AddVariableValue(new AttributeVariableValue("Red", valueId));

        // Act
        var result = attribute.RemoveVariableValue(valueId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        attribute.VariableValues.Should().BeEmpty();
    }

    [Fact]
    public void RemoveVariableValue_Should_Return_Ok_When_Value_Does_Not_Exist()
    {
        // Arrange
        var attribute = CreateValid(variable: true);

        // Act
        var result = attribute.RemoveVariableValue(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}