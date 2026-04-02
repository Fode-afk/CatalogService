using CatalogService.Domain.Errors;
using CatalogService.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.ValueObjects;

public sealed partial class SeoMetadata : ValueObject
{
    public const int TitleMaxLength = 60;
    public const int DescriptionMaxLength = 160;
    public const int KeywordsMaxLength = 255;

    public string Title { get; }
    public string Description { get; }
    public string Keywords { get; }

    private SeoMetadata(
        string title,
        string description,
        string keywords)
    {
        Title = title;
        Description = description;
        Keywords = keywords;
    }

    public static IResult<SeoMetadata> Create(
        string title,
        string description,
        string keywords)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Fail<SeoMetadata>(SeoMetadataErrors.TitleNullOrEmpty());

        if (string.IsNullOrWhiteSpace(description))
            return Fail<SeoMetadata>(SeoMetadataErrors.DescriptionNullOrEmpty());

        if (string.IsNullOrEmpty(keywords))
            return Fail<SeoMetadata>(SeoMetadataErrors.KeywordsNullOrEmpty());

        title = Normalize(title);
        description = Normalize(description);
        keywords = NormalizeKeywords(keywords);

        if (title.Length > TitleMaxLength)
            return Fail<SeoMetadata>(SeoMetadataErrors.TitleTooLong());

        if (description.Length > DescriptionMaxLength)
            return Fail<SeoMetadata>(SeoMetadataErrors.DescriptionTooLong());

        if (keywords.Length > KeywordsMaxLength)
            return Fail<SeoMetadata>(SeoMetadataErrors.KeywordsTooLong());

        return Ok(new SeoMetadata(title, description, keywords));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Title;
        yield return Description;
        yield return Keywords;
    }

    public override string ToString()
        => $"{Title} | {Description}";

    public static implicit operator string(SeoMetadata seoMetadata) => seoMetadata.ToString();

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();

        value = NormalizeRegex().Replace(value, " ");

        return value;
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

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeRegex();
}
