namespace CatalogService.Domain.Snapshots;

public sealed class ProductPriceSnapshot
{
    public Guid ProductId { get; set; }
    public decimal? PriceAmount { get; private set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }

    public bool HasPrice => PriceAmount is not null && PriceAmount > 0;
}