using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using CatalogService.Domain.Errors;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class Tag : ValueObject
{
    public static int MaxLength => 50;

    public string Value { get; }

    private Tag(string value)
    {
        Value = value;
    }

    public static IResult<Tag> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Tag>(TagErrors.Empty());

        value = value.Trim();

        if (value.Length > MaxLength)
            return Fail<Tag>(TagErrors.TooLong());

        return Ok(new Tag(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Tag tag) => tag.ToString();
}
