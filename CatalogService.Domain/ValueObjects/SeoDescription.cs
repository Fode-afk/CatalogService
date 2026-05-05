using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed partial class SeoDescription : ValueObject
{
    public const int MaxLength = 160;

    public string Value { get; }

    private SeoDescription(string value)
    {
        Value = value;
    }

    public static IResult<SeoDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<SeoDescription>(SeoDescriptionErrors.NullOrEmpty());

        value = Normalize(value);

        if (value.Length > MaxLength)
            return Fail<SeoDescription>(SeoDescriptionErrors.TooLong());

        return Ok(new SeoDescription(value));
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();

        value = NormalizeRegex().Replace(value, " ");

        return value;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeRegex();

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
