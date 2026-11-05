using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class Description : ValueObject
{
    public static int MaxLength => 5000;

    public string Value { get; }

    private Description(string value) => Value = value;

    public static IResult<Description> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Description>(DescriptionErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<Description>(DescriptionErrors.TooLong(MaxLength));

        return Ok(new Description(value));
    }

    public override IEnumerable<object> GetAtomicValues() => [Value];

    public override string ToString() => Value;
    public static implicit operator string(Description description) => description.ToString();
}
