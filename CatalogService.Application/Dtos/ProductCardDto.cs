using migApp.Shared.Enums.Inventory;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Application.Dtos;

public sealed record ProductCardDto(
    string Id,
    string Name,
    string Slug,
    string Description,
    string ShortDescription,

    decimal RatingAvg,
    int RatingCount,

    string? DefaultProductId,
    int ProductCount,

    long PriceMinorAmount,
    long OldPriceMinorAmount,

    StockStatus StockStatus,

    string CategoryId,
    string CategoryName,
    string CategorySlug,

    string VendorId,
    string Brand,

    ProductCardStatus ProductCardStatus,

    SeoMetadataDto SeoMetadata,

    Dictionary<string, string> Attributes,
    IEnumerable<string> Tags,
    IEnumerable<ProductCardImageDto> Images);