using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class ShortDescription : ValueObject
{
    public static int MaxLength => 500;

    public string Value { get; }

    private ShortDescription(string value)
    {
        Value = value;
    }

    public static IResult<ShortDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<ShortDescription>(ShortDescriptionErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<ShortDescription>(ShortDescriptionErrors.TooLong(MaxLength));

        return Ok(new ShortDescription(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ShortDescription description) => description.ToString();
}
