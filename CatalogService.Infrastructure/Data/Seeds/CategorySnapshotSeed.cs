using CatalogService.Domain.Snapshots;

namespace CatalogService.Infrastructure.Data.Seeds;

internal static class CategorySnapshotSeed
{
    public static List<CategorySnapshot> Data =
    [
        new CategorySnapshot
        {
            CategoryId = Guid.Parse("50000000-0000-0000-0000-000000000005"),
            IsActive = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            Version = 1
        }
    ];
}
