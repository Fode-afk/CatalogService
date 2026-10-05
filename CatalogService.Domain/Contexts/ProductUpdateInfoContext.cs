using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductUpdateInfoContext(
    bool VendorIsActive,
    bool CanEditContent,
    bool CategoryIsActive,
    bool BrandIsAssignable) :
        IVendorContext,
        IProductContext,
        ICategoryContext, 
        IBrandContext;