using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<ProductCard> ProductCards { get; }
    DbSet<ProductCardImage> ProductCardImages { get; }
    DbSet<ProductCardReadModel> ProductCardReadModels { get; }

    DbSet<VendorSnapshot> VendorSnapshots { get; }
    DbSet<ProductSnapshot> ProductSnapshots { get; }
    DbSet<ProductPriceSnapshot> ProductPriceSnapshots { get; }
    DbSet<ProductInventorySnapshot> ProductInventorySnapshots { get; }
    DbSet<CategorySnapshot> CategorySnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
