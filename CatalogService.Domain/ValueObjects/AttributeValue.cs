using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class AttributeValue : ValueObject
{
    public const int MaxLength = 500;

    public string Value { get; }

    private AttributeValue(string value)
    {
        Value = value;
    }

    public static IResult<AttributeValue> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<AttributeValue>(AttributeValueErrors.NullOrEmpty());

        if (value.Length > MaxLength)
            return Fail<AttributeValue>(AttributeValueErrors.TooLong());

        return Ok(new AttributeValue(value.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(AttributeValue attributeValue) => attributeValue.ToString();
}
