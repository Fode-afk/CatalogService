using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace CatalogService.Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }

    DbSet<VendorSnapshot> VendorSnapshots { get; }
    DbSet<ProductVariantSnapshot> ProductVariantSnapshots { get; }
    DbSet<ProductVariantPriceSnapshot> ProductVariantPriceSnapshots { get; }
    DbSet<CategorySnapshot> CategorySnapshots { get; }
    DbSet<BrandSnapshot> BrandSnapshots { get; }
    DbSet<CharacteristicSnapshot> CharacteristicSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    void ClearChangeTracker();
}
