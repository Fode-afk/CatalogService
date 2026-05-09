using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class AttributeGroupName : ValueObject
{
    public static int MaxLength => 100;
    public string Value { get; }

    private AttributeGroupName(string value) => Value = value;

    public static IResult<AttributeGroupName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<AttributeGroupName>(AttributeGroupNameErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<AttributeGroupName>(AttributeGroupNameErrors.TooLong());

        return Ok(new AttributeGroupName(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(AttributeGroupName attributeGroupName) => attributeGroupName.ToString();
}
