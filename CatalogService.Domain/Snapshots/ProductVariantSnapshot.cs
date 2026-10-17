namespace CatalogService.Domain.Snapshots;

public sealed class ProductVariantSnapshot
{
    public Guid ProductVariantId { get; init; }
    public Guid ProductId { get; init; }
    public bool HasMainImage { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}