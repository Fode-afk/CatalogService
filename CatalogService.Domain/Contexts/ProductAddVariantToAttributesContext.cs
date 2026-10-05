using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductAddVariantToAttributesContext(
    bool VendorIsActive,
    bool CanEditContent) : IVendorContext, IProductContext;