using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantPriceSnapshotConfiguration : IEntityTypeConfiguration<ProductVariantPriceSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductVariantPriceSnapshot> builder)
    {
        builder.ToTable("ProductVariantPriceSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.ProductVariantId);

        builder.HasIndex(x => x.ProductId);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}