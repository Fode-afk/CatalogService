using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductCardConfiguration : IEntityTypeConfiguration<ProductCard>
{
    public void Configure(EntityTypeBuilder<ProductCard> builder)
    {
        builder.ToTable("ProductCards", Schemas.CatalogWrite);

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.VendorId);
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.DefaultProductId);
        builder.HasIndex(p => p.ProductCardStatus);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.UpdatedAt);

        builder.HasIndex(p => new
        {
            p.CategoryId,
            p.ProductCardStatus
        });

        builder.HasIndex(p => new
        {
            p.VendorId,
            p.ProductCardStatus
        });

        builder.Property(p => p.Name)
            .HasMaxLength(Name.MaxLength)
            .HasConversion(
                name => name.Value,
                value => Name.Create(value).Value);

        builder.Property(p => p.Slug)
            .HasMaxLength(Slug.MaxLength)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value).Value);

        builder.Property(p => p.Description)
            .HasMaxLength(Description.MaxLength)
            .HasConversion(
                description => description.Value,
                value => Description.Create(value).Value);

        builder.Property(p => p.ShortDescription)
            .HasMaxLength(ShortDescription.MaxLength)
            .HasConversion(
                shortDescription => shortDescription.Value,
                value => ShortDescription.Create(value).Value);

        builder.Property(x => x.ProductCount)
            .HasConversion(
                count => count.Value,
                value => ProductCount.Create(value).Value);

        builder.Property(p => p.Brand)
            .HasMaxLength(Brand.MaxLength)
            .HasConversion(
                brand => brand.Value,
                value => Brand.Create(value).Value);

        builder.OwnsOne(p => p.SeoMetadata, seoBuilder =>
        {  
            seoBuilder.Property(m => m.Title)
                .HasMaxLength(SeoTitle.MaxLength)
                .HasColumnName("SeoTitle")
                .HasConversion(
                    title => title.Value,
                    value => SeoTitle.Create(value).Value);

            seoBuilder.Property(m => m.Description)
                .HasMaxLength(SeoDescription.MaxLength)
                .HasColumnName("SeoDescription")
                .HasConversion(
                    description => description.Value,
                    value => SeoDescription.Create(value).Value);

            seoBuilder.Property(m => m.Keywords)
                .HasMaxLength(SeoKeywords.MaxLength)
                .HasColumnName("SeoKeywords")
                .HasConversion(
                    keywords => keywords.Value,
                    value => SeoKeywords.Create(value).Value);
        });

        builder.OwnsMany(p => p.Attributes, a =>
        {
            a.WithOwner().HasForeignKey("ProductId");

            a.ToTable("ProductCardAttributes");

            a.Property<int>("Id");
            a.HasKey("Id");

            a.OwnsOne(x => x.Name, n =>
            {
                n.Property(x => x.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(AttributeName.MaxLength);
            });

            a.OwnsOne(x => x.Value, v =>
            {
                v.Property(x => x.Value)
                    .HasColumnName("Value")
                    .HasMaxLength(AttributeValue.MaxLength);
            });
        });

        builder.OwnsMany(p => p.Tags, t =>
        {
            t.WithOwner().HasForeignKey("ProductId");

            t.ToTable("ProductCardTags");

            t.Property<int>("Id");
            t.HasKey("Id");

            t.Property(x => x.Value)
                .HasColumnName("Tag")
                .HasMaxLength(Tag.MaxLength);
        });

        builder.Property(x => x.RowVersion)
           .IsRowVersion()
           .IsConcurrencyToken();

        builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey("ProductCardId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
