namespace CatalogService.Application.Dtos;

public sealed record ProductCardImageDto(
    string Url,
    string Alt,
    bool IsMain,
    int SortOrder);