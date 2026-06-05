using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class Name : ValueObject
{
    public static int MaxLength => 100;
    public static int MinLength => 3;

    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static IResult<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Name>(NameErrors.NullOrEmpty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<Name>(NameErrors.TooLong(MaxLength));

        if (value.Length < MinLength)
            return Fail<Name>(NameErrors.TooShort(MinLength));

        return Ok(new Name(value));
    }

    public static string Normalize(Name name) =>
        name.Value
            .ToLower()
            .Replace(" ", "")
            .Trim();

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.ToString();
}
