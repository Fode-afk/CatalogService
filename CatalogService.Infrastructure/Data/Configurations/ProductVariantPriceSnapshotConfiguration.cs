using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductVariantPriceSnapshotConfiguration : IEntityTypeConfiguration<ProductVariantPriceSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductVariantPriceSnapshot> builder)
    {
        builder.ToTable("ProductVariantPriceSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.PriceId);

        builder.HasIndex(x => x.ProductVariantId);
        builder.HasIndex(x => x.Market).IsUnique();

        builder.Property(p => p.Market)
            .HasMaxLength(Market.CodeLength)
            .HasConversion(
                market => market.Code,
                value => Market.Create(value).Value);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}