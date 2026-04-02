using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class Name : ValueObject
{
    private const int MaxLength = 40;
    private const int MinLength = 3;

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

        if (value.Length > MaxLength)
            return Fail<Name>(NameErrors.TooLong());

        if (value.Length < MinLength)
            return Fail<Name>(NameErrors.TooShort());

        return Ok(new Name(value));
    }

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.ToString();
}
