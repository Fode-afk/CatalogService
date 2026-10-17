namespace CatalogService.Domain.Snapshots;

public sealed class CategorySnapshot
{
    public Guid CategoryId { get; init; }
    public bool IsActive { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}