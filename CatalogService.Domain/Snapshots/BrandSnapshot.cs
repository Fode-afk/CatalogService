namespace CatalogService.Domain.Snapshots;

public sealed class BrandSnapshot
{
    public Guid BrandId { get; init; }
    public bool IsAssignable { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}