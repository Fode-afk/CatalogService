using CatalogService.Domain.Abstractions;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardCreationContext(
    bool VendorIsActive,
    bool CategoryIsActive,
    IReadOnlyCollection<ProductCardAttribute> Attributes,
    IReadOnlyCollection<Tag> Tags) :
        IVendorContext,
        IAttributesContext,
        ITagsContext;