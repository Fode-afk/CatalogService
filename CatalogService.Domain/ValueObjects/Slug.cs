using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed partial class Slug : ValueObject
{
    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled)]
    private static partial Regex SlugRegex();
    private const int MaxLength = 100;
    private const int MinLength = 3;


    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static IResult<Slug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Slug>(SlugErrors.NullOrEmpty());

        if (value.Length > MaxLength)
            return Fail<Slug>(SlugErrors.TooLong());

        if (value.Length < MinLength)
            return Fail<Slug>(SlugErrors.TooShort());

        value = value.Trim().ToLowerInvariant();

        if (!SlugRegex().IsMatch(value))
            return Fail<Slug>(SlugErrors.InvalidFormat());

        return Ok(new Slug(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.ToString();
}
