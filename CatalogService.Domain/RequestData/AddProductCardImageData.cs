using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.RequestData;

public sealed record AddProductCardImageData(
    ImageUrl Url,
    AltText Alt);