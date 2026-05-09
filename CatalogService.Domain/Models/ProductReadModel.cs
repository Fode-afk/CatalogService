using migApp.Shared.Enums.Inventory;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Models;

public sealed class ProductReadModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string NameNormalized { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required string ShortDescription { get; set; }

    public decimal RatingAvg { get; set; }
    public int RatingCount { get; set; }

    public Guid? DefaultProductId { get; set; }
    public int ProductCount { get; set; }

    public decimal? PriceAmount { get; set; }
    public decimal? OldPriceAmount { get; set; }
    public DateTimeOffset? PriceUpdatedAt { get; set; }

    public StockStatus StockStatus { get; set; }
    public DateTimeOffset? StockUpdatedAt { get; set; }

    public Guid CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public required string CategorySlug { get; set; }

    public Guid VendorId { get; set; }
    public required string VendorName { get; set; }

    public Guid BrandId { get; set; }
    public required string BrandName { get; set; }

    public ProductStatus ProductStatus { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public required string SeoTitle { get; set; }
    public required string SeoDescription { get; set; }
    public required string SeoKeywords { get; set; }

    public string? MainImage { get; set; }

    public required string AttributesJson { get; set; }
    public required string TagsJson { get; set; }
    public required string TagsFlat { get; set; }
    public string? ImagesJson { get; set; }
}
