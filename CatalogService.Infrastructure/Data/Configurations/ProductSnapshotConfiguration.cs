using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductSnapshotConfiguration : IEntityTypeConfiguration<ProductSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductSnapshot> builder)
    {
        builder.ToTable("ProductSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.ProductId);
        builder.HasIndex(x => x.ProductCardId);

        builder.Property<byte[]>("RowVersion")
           .IsRowVersion()
           .IsConcurrencyToken();
    }
}
