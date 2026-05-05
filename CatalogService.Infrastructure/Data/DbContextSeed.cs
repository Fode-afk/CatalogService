using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using migApp.Shared.Enums.Vendors;

namespace CatalogService.Infrastructure.Data;

internal static class DbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.VendorSnapshots.Any())
        {
            context.VendorSnapshots.Add(
                new VendorSnapshot
                {
                    VendorId = Guid.Parse("3DA8CA23-4511-42CF-8AD0-03718D60CE9F"),
                    Status = VendorStatus.Active,
                    IsVerified = true,
                    Version = 0,
                    UpdatedAt = DateTime.UtcNow
                });
        }

        if (!context.CategorySnapshots.Any())
        {
            context.CategorySnapshots.Add(
                new CategorySnapshot
                {
                    CategoryId = Guid.Parse("3DA8CA23-4511-42CF-8AD0-03718D60CE9D"),
                    IsActive = false,
                    Version = 0,
                    UpdatedAt = DateTime.UtcNow
                });
        }

        await context.SaveChangesAsync();
    }
}
