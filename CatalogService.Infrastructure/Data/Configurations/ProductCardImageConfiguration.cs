using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductCardImageConfiguration : IEntityTypeConfiguration<ProductCardImage>
{
    public void Configure(EntityTypeBuilder<ProductCardImage> builder)
    {
        builder.ToTable("ProductCardImages", Schemas.CatalogWrite);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
            .HasConversion(
                url => url.Value,
                value => ImageUrl.Create(value).Value);

        builder.Property(i => i.Alt)
            .HasMaxLength(AltText.MaxLength)
            .HasConversion(
                alt => alt.Value,
                value => AltText.Create(value).Value);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
