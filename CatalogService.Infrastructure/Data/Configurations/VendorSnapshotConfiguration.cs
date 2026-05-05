using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class VendorSnapshotConfiguration : IEntityTypeConfiguration<VendorSnapshot>
{
    public void Configure(EntityTypeBuilder<VendorSnapshot> builder)
    {
        builder.ToTable("VendorSnapshots", Schemas.CatalogWrite);

        builder.HasKey(v => v.VendorId);

        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.IsVerified);
        builder.HasIndex(v => v.UpdatedAt);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}