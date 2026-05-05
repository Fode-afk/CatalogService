namespace CatalogService.Application.Dtos;

public sealed record SeoMetadataDto(
    string Title,
    string Description,
    string Keywords);