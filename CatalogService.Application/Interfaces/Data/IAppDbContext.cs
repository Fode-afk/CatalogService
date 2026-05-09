using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductReadModel> ProductReadModels { get; }

    DbSet<VendorSnapshot> VendorSnapshots { get; }
    DbSet<ProductVariantSnapshot> ProductVariantSnapshots { get; }
    DbSet<ProductVariantPriceSnapshot> ProductVariantPriceSnapshots { get; }
    DbSet<CategorySnapshot> CategorySnapshots { get; }
    DbSet<BrandSnapshot> BrandSnapshots { get; }
    DbSet<CharacteristicSnapshot> CharacteristicSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
