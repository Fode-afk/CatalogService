using CatalogService.Infrastructure.Data.Seeds;

namespace CatalogService.Infrastructure.Data;

internal static class DbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.VendorSnapshots.Any())
            context.AddRange(VendorSnapshotSeed.Data);

        if (!context.CategorySnapshots.Any())
            context.AddRange(CategorySnapshotSeed.Data);

        if (!context.BrandSnapshots.Any())
            context.AddRange(BrandSnapshotSeed.Data);

        if (!context.CharacteristicSnapshots.Any())
            context.AddRange(CharacteristicSnapshotSeed.Data);

        //if (!context.VariationSnapshots.Any())
        //    context.AddRange(ProductVariantSnapshotSeed.Data);

        await context.SaveChangesAsync();
    }
}