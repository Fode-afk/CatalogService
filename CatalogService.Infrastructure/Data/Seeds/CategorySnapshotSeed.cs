using CatalogService.Domain.Snapshots;

namespace CatalogService.Infrastructure.Data.Seeds;

internal static class CategorySnapshotSeed
{
    public static readonly List<CategorySnapshot> Data =
    [
        // =========================
        // ACTIVE CATEGORIES
        // =========================

        new()
        {
            CategoryId = Guid.Parse("60000000-0000-0000-0000-000000000001"),
            IsActive = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-90),
            Version = 1
        },

        new()
        {
            CategoryId = Guid.Parse("60000000-0000-0000-0000-000000000002"),
            IsActive = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-45),
            Version = 2
        },

        new()
        {
            CategoryId = Guid.Parse("60000000-0000-0000-0000-000000000003"),
            IsActive = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-7),
            Version = 5
        },

        // =========================
        // INACTIVE CATEGORIES
        // =========================

        new()
        {
            CategoryId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
            IsActive = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-120),
            Version = 1
        },
    ];
}
