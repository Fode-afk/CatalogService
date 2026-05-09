using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductUnpublishContext(
    bool VendorIsActive,
    bool CanBeModified) : IVendorContext, IProductContext;