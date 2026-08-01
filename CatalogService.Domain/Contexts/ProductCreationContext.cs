using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCreationContext(
    bool VendorIsActive,
    bool CategoryIsActive,
    bool BrandIsAssignable) :
        IVendorContext,
        ICategoryContext,
        IBrandContext;