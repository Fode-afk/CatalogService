using migApp.Shared.Domain.Primitives;

namespace CatalogService.Domain.ValueObjects;

public sealed class SeoMetadata : ValueObject
{
    public SeoTitle Title { get; }
    public SeoDescription Description { get; }
    public SeoKeywords Keywords { get; }

    private SeoMetadata(
        SeoTitle title,
        SeoDescription description,
        SeoKeywords keywords)
    {
        Title = title;
        Description = description;
        Keywords = keywords;
    }

    public static SeoMetadata Create(
        SeoTitle title,
        SeoDescription description,
        SeoKeywords keywords) =>
        new(title, description, keywords);

    public override IEnumerable<object> GetAtomicValues() => [Title, Description, Keywords];
}