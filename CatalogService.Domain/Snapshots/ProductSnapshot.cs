namespace CatalogService.Domain.Snapshots;

public sealed class ProductSnapshot
{
    public Guid ProductId { get; private set; }
    public Guid ProductCardId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long Version { get; set; }
}
