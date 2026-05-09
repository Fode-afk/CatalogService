using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductDeleteContext(
    bool VendorIsActive,
    bool CanBeModified) : IVendorContext, IProductContext;