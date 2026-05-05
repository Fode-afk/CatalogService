using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.RequestData;

public sealed record ProductCardUpdateInfoData(
    Name Name,
    Description Description,
    ShortDescription ShortDescription,
    Brand Brand,
    SeoMetadata SeoMetadata);