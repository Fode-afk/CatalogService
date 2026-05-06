using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class Description : ValueObject
{
    public static int MaxLength => 3000;

    private Description(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static IResult<Description> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Description>(DescriptionErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<Description>(DescriptionErrors.TooLong());

        return Ok(new Description(value));
    }

    public override string ToString() => Value;

    public static implicit operator string(Description description) => description.ToString();
}
