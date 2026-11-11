using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Domain.Contexts;

public sealed record ProductSubmitForPublishContext(
    bool VendorIsActive,
    bool CategoryIsActive,
    bool BrandIsAssignable,
    bool CanEditContent,
    bool AllVariantsPricedInAllMarkets,
    IReadOnlyCollection<ProductAttribute> Attributes,
    IReadOnlyCollection<Tag> Tags,
    IReadOnlyCollection<Market> Markets,
    IReadOnlyList<ProductVariantSnapshot> VariationSnapshots) :
        IVendorContext,
        IProductContext,
        ICategoryContext,
        IBrandContext,
        IAttributesContext,
        ITagsContext,
        IMarketsContext;