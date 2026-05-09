using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Contexts;

public sealed record ProductPublishContext(
    bool VendorIsActive,
    bool CategoryIsActive,
    bool BrandIsActive,
    bool CanBeModified,
    IReadOnlyCollection<ProductAttribute> Attributes,
    IReadOnlyCollection<Tag> Tags,
    List<ProductVariantSnapshot> VariationSnapshots,
    List<ProductVariantPriceSnapshot> PriceSnapshots) :
        IVendorContext,
        IProductContext,
        ICategoryContext,
        IBrandContext,
        IAttributesContext,
        ITagsContext;