using CatalogService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductReadModelConfiguration : IEntityTypeConfiguration<ProductReadModel>
{
    public void Configure(EntityTypeBuilder<ProductReadModel> builder)
    {
        builder.ToTable("ProductReadModels", Schemas.CatalogRead);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.CategorySlug);
        builder.HasIndex(x => x.CategoryName);
        builder.HasKey(x => x.BrandId);
        builder.HasIndex(x => x.VendorId);
        builder.HasIndex(x => x.DefaultProductId);
        builder.HasIndex(x => x.StockStatus);
        builder.HasIndex(x => x.PriceAmount);
        builder.HasIndex(x => x.PriceUpdatedAt);
        builder.HasIndex(x => x.StockUpdatedAt);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.UpdatedAt);
        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => new
        {
            x.CategoryId,
            x.StockStatus
        });

        builder.HasIndex(x => new
        {
            x.CategoryId,
            x.PriceAmount
        });

        builder.HasIndex(x => new
        {
            x.VendorId,
            x.StockStatus
        });

        builder.HasIndex(x => new
        {
            x.BrandId,
            x.CategoryId
        });

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode();

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .IsUnicode();

        builder.Property(x => x.ShortDescription)
            .IsRequired()
            .HasMaxLength(1000)
            .IsUnicode();

        builder.Property(x => x.RatingAvg)
            .HasPrecision(3, 2);

        builder.Property(x => x.RatingCount);

        builder.Property(x => x.PriceAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.OldPriceAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.CategoryName)
            .HasMaxLength(200);

        builder.Property(x => x.CategorySlug)
            .HasMaxLength(200);

        builder.Property(x => x.SeoTitle)
            .IsRequired()
            .HasMaxLength(250)
            .IsUnicode();

        builder.Property(x => x.SeoDescription)
            .IsRequired()
            .HasMaxLength(1000)
            .IsUnicode();

        builder.Property(x => x.SeoKeywords)
            .IsRequired()
            .HasMaxLength(1000)
            .IsUnicode();

        builder.Property(x => x.MainImage)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.AttributesJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TagsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TagsFlat)
            .IsRequired()
            .HasMaxLength(4000)
            .IsUnicode();

        builder.Property(x => x.ImagesJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
