using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductArchiveContext(
    bool VendorIsActive,
    bool CanBeModified) : IVendorContext, IProductContext;