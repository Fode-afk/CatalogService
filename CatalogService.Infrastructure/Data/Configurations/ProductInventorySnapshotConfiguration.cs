using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductInventorySnapshotConfiguration : IEntityTypeConfiguration<ProductInventorySnapshot>
{
    public void Configure(EntityTypeBuilder<ProductInventorySnapshot> builder)
    {
        builder.ToTable("ProductInventorySnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.ProductId);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}
