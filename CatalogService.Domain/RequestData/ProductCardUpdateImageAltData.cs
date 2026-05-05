using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.RequestData;

public sealed record ProductCardUpdateImageAltData(
    ImageUrl Url,
    AltText Alt);