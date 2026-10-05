using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductRemoveVariantFromAttributesContext(
    bool VendorIsActive,
    bool CanEditContent) : IVendorContext, IProductContext;