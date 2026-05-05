using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductPriceSnapshotConfiguration : IEntityTypeConfiguration<ProductPriceSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductPriceSnapshot> builder)
    {
        builder.ToTable("ProductPriceSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.ProductId);

        builder.Property(x => x.PriceAmount)
            .HasPrecision(18, 2);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}
