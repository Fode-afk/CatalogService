using CatalogService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed class SeoKeywords : ValueObject
{
    public const int MaxLength = 255;

    public string Value { get; }

    private SeoKeywords(string value)
    {
        Value = value;
    }

    public static IResult<SeoKeywords> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<SeoKeywords>(SeoKeywordsErrors.NullOrEmpty());

        value = NormalizeKeywords(value);

        if (value.Length > MaxLength)
            return Fail<SeoKeywords>(SeoKeywordsErrors.TooLong());

        return Ok(new SeoKeywords(value));
    }

    private static string NormalizeKeywords(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var keywords = value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim().ToLowerInvariant())
            .Distinct();

        return string.Join(", ", keywords);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
