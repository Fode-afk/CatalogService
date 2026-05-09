using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class CategorySnapshotConfiguration : IEntityTypeConfiguration<CategorySnapshot>
{
    public void Configure(EntityTypeBuilder<CategorySnapshot> builder)
    {
        builder.ToTable("CategorySnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.CategoryId);

        builder.HasIndex(x => x.IsActive);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
