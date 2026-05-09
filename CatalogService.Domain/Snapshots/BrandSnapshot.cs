namespace CatalogService.Domain.Snapshots;

public sealed class BrandSnapshot
{
    public Guid BrandId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}