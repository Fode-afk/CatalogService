using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class BrandSnapshotConfiguration : IEntityTypeConfiguration<BrandSnapshot>
{
    public void Configure(EntityTypeBuilder<BrandSnapshot> builder)
    {
        builder.ToTable("BrandSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.BrandId);

        builder.HasIndex(x => x.IsAssignable);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
