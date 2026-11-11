using CatalogService.Domain.Snapshots;
using CatalogService.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class VendorSnapshotConfiguration : IEntityTypeConfiguration<VendorSnapshot>
{
    public void Configure(EntityTypeBuilder<VendorSnapshot> builder)
    {
        builder.ToTable("VendorSnapshots", Schemas.CatalogWrite);

        builder.HasKey(v => v.VendorId);

        builder.PrimitiveCollection(p => p.Markets)
            .ElementType()
            .HasConversion(new MarketConverter())
            .HasMaxLength(Market.CodeLength);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}