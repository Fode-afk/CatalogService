using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.RequestData;

public sealed record ProductUpdateInfoData(
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    SeoMetadata SeoMetadata);