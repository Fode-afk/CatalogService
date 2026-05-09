using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.RequestData;

public sealed record ProductCreationData(
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    SeoMetadata SeoMetadata);