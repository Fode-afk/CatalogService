namespace CatalogService.Domain.Snapshots;

public sealed class ProductVariantPriceSnapshot
{
    public Guid ProductVariantId { get; init; }
    public Guid ProductId { get; init; }
    public bool HasPrice { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}