using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", Schemas.CatalogWrite);

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.VendorId);
        builder.HasIndex(p => p.BrandId);
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.ProductStatus);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.UpdatedAt);
        builder.HasIndex(p => p.IsDeleted);

        builder.HasIndex(p => new 
        { 
            p.IsDeleted,
            p.ProductStatus 
        });

        builder.HasIndex(p => new
        {
            p.CategoryId,
            p.ProductStatus
        });

        builder.HasIndex(p => new
        {
            p.VendorId,
            p.ProductStatus
        });

        builder.Property(p => p.Name)
            .HasMaxLength(ProductName.MaxLength)
            .HasConversion(
                name => name.Value,
                value => ProductName.Create(value).Value);

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

        builder.Property(p => p.ProductStatus)
            .HasConversion<string>()
            .HasMaxLength(100);

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
            a.ToTable("ProductAttributes", Schemas.CatalogWrite);

            a.Property<int>("Id").ValueGeneratedOnAdd();
            a.HasKey("Id");

            a.Property(x => x.CharacteristicId);
            a.Property(x => x.IsVariable);
            a.Property(x => x.IsUnifying);

            a.Property(x => x.CharType)
                .HasConversion<string>()
                .HasMaxLength(50);

            a.OwnsOne(x => x.Name, n =>
            {
                n.Property(x => x.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(AttributeName.MaxLength);
            });

            a.Property(x => x.Value)
                .HasMaxLength(AttributeValue.MaxLength)
                .HasConversion(
                    value => value != null ? value.Value : null,
                    value => value != null ? AttributeValue.Create(value).Value : null);

            a.Property(x => x.GroupName)
                .HasMaxLength(AttributeGroupName.MaxLength)
                .IsRequired(false)
                .HasConversion(
                    g => g != null ? g.Value : null,
                    value => value != null ? AttributeGroupName.Create(value).Value : null);

            a.OwnsMany(x => x.VariableValues, v =>
            {
                v.WithOwner().HasForeignKey("AttributeId");
                v.ToTable("ProductAttributeVariableValues", Schemas.CatalogWrite);

                v.Property<int>("AttributeId");
                v.Property(x => x.ValueId).IsRequired();
                v.Property(x => x.Value)
                    .HasMaxLength(500)
                    .IsRequired();

                v.HasKey("AttributeId", nameof(AttributeVariableValue.ValueId));
            });

            a.Navigation(x => x.VariableValues)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.OwnsMany(p => p.Tags, t =>
        {
            t.WithOwner().HasForeignKey("ProductId");

            t.ToTable("ProductTags");

            t.Property<int>("Id");
            t.HasKey("Id");

            t.Property(x => x.Value)
                .HasColumnName("Tag")
                .HasMaxLength(Tag.MaxLength);
        });

        builder.OwnsMany(p => p.SuspensionReasons, s =>
        {
            s.WithOwner().HasForeignKey("ProductId");
            s.ToTable("ProductSuspensionReasons", Schemas.CatalogWrite);

            s.Property<int>("Id");
            s.HasKey("Id");

            s.Property(x => x.Reason)
                .HasConversion<string>()
                .HasMaxLength(100);
        });

        builder.Property(p => p.RejectionReason)
            .HasMaxLength(RejectionReason.MaxLength)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? RejectionReason.Create(value).Value : null);

        builder.Property(p => p.BlockReason)
            .HasMaxLength(BlockReason.MaxLength)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? BlockReason.Create(value).Value : null);

        builder.Navigation(p => p.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.SuspensionReasons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Ignore(p => p.IsDeleted);

        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}