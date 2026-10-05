using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Contexts;

public sealed record ProductSubmitForPublishContext(
    bool VendorIsActive,
    bool CategoryIsActive,
    bool BrandIsAssignable,
    bool CanEditContent,
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