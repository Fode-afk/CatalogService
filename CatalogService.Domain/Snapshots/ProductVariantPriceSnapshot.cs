using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Domain.Snapshots;

public sealed class ProductVariantPriceSnapshot
{
    public Guid PriceId { get; init; }
    public Guid ProductVariantId { get; init; }
    public required Market Market { get; init; }
}