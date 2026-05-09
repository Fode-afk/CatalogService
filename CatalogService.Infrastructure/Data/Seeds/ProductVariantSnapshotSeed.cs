using CatalogService.Domain.Snapshots;

namespace CatalogService.Infrastructure.Data.Seeds;

internal static class ProductVariantSnapshotSeed
{
    public static readonly List<ProductVariantSnapshot> Data =
    [
        // =========================
        // iPhone 17 Variations
        // ProductId:
        // 3034F284-7FFA-4000-A0C6-EEFBAFD4D088
        // =========================
        
        new()
        {
            ProductVariantId = Guid.Parse("D1111111-1111-1111-1111-111111111111"),
            ProductId = Guid.Parse("3034F284-7FFA-4000-A0C6-EEFBAFD4D088"),
            HasMainImage = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-10),
            Version = 1,       
        },
        
        new()
        {
            ProductVariantId = Guid.Parse("D1111111-1111-1111-1111-111111111112"),
            ProductId = Guid.Parse("3034F284-7FFA-4000-A0C6-EEFBAFD4D088"),
            HasMainImage = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-8),
            Version = 1,          
        },
        
        new()
        {
            ProductVariantId = Guid.Parse("D1111111-1111-1111-1111-111111111113"),
            ProductId = Guid.Parse("3034F284-7FFA-4000-A0C6-EEFBAFD4D088"),
            HasMainImage = true,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-6),
            Version = 2,      
        },
        
        new()
        {
            ProductVariantId = Guid.Parse("D1111111-1111-1111-1111-111111111114"),
            ProductId = Guid.Parse("3034F284-7FFA-4000-A0C6-EEFBAFD4D088"),
            HasMainImage = false,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-4),
            Version = 2,      
        }
    ];
}
