using CatalogService.Domain.Errors;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Characteristics;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Models;

public sealed class ProductAttribute
{
    private ProductAttribute() { }
    private ProductAttribute(
        Guid? characteristicId,
        AttributeName name,
        AttributeValue? value,
        AttributeCharType charType,
        AttributeGroupName? groupName,
        bool isVariable,
        bool isUnifying)
    {
        CharacteristicId = characteristicId;
        Name = name;
        Value = value;
        CharType = charType;
        GroupName = groupName;
        IsVariable = isVariable;
        IsUnifying = isUnifying;
    }

    public Guid? CharacteristicId { get; private set; }
    public AttributeName Name { get; private set; }
    public AttributeValue? Value { get; private set; }
    public AttributeCharType CharType { get; private set; }
    public AttributeGroupName? GroupName { get; private set; }
    public bool IsVariable { get; private set; }
    public bool IsUnifying { get; private set; }

    private readonly List<AttributeVariableValue> _variableValues = [];
    public IReadOnlyList<AttributeVariableValue> VariableValues => _variableValues;

    public static IResult<ProductAttribute> Create(
        Guid? characteristicId,
        AttributeName name,
        AttributeValue value,
        AttributeCharType charType,
        AttributeGroupName? groupName = null,
        bool isUnifying = false)
    {
        return Ok(new ProductAttribute(
            characteristicId,
            name,
            value,
            charType,
            groupName,
            isVariable: false,
            isUnifying));
    }

    public static IResult<ProductAttribute> CreateVariable(
        Guid? characteristicId,
        AttributeName name,
        AttributeCharType charType,
        AttributeGroupName? groupName = null)
    {
        return Ok(new ProductAttribute(
            characteristicId,
            name,
            value: null,
            charType,
            groupName,
            isVariable: true,
            isUnifying: false));
    }

    public void MarkAsVariable()
    {
        IsVariable = true;
        Value = null;
    }

    public IResult AddVariableValue(AttributeVariableValue variableValue)
    {
        if (!IsVariable)
            return Fail(ProductAttributeErrors.NotVariable());

        if (_variableValues.Any(v => v.ValueId == variableValue.ValueId))
            return Ok();

        _variableValues.Add(variableValue);
        return Ok();
    }

    public IResult RemoveVariableValue(Guid variantId)
    {
        var existing = _variableValues.FirstOrDefault(v => v.ValueId == variantId);
        if (existing is null)
            return Ok();

        _variableValues.Remove(existing);

        return Ok();
    }
}

public sealed record AttributeVariableValue(string Value, Guid? ValueId);