using CatalogService.Domain.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class CharacteristicSnapshotConfiguration : IEntityTypeConfiguration<CharacteristicSnapshot>
{
    public void Configure(EntityTypeBuilder<CharacteristicSnapshot> builder)
    {
        builder.ToTable("CharacteristicSnapshots", Schemas.CatalogWrite);

        builder.HasKey(x => x.CharacteristicId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.GroupName)
            .HasMaxLength(256);

        builder.Property(x => x.CharType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(x => x.CategoryId);

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}