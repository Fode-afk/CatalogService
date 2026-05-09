using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantSnapshotConfiguration : IEntityTypeConfiguration<ProductVariantSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductVariantSnapshot> builder)
    {
        builder.ToTable("ProductVariantSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.ProductVariantId);
        builder.HasIndex(x => x.ProductId);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}
