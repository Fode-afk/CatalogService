using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.RequestData;

public sealed record ProductCardCreationData(
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    Brand Brand,
    SeoMetadata SeoMetadata,
    IReadOnlyCollection<ProductCardAttribute> Attributes,
    IReadOnlyCollection<Tag> Tags);