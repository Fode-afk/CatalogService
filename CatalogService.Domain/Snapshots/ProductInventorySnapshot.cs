using migApp.Shared.Enums.Inventory;

namespace CatalogService.Domain.Snapshots;

public sealed class ProductInventorySnapshot
{
    public Guid ProductId { get; set; }
    public int StockQuantity { get; set; }
    public StockStatus StockStatus { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }

    public bool InStock => StockQuantity > 0 && StockStatus == StockStatus.InStock;
}
