using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardArchivedContext(
    bool VendorIsActive) : IVendorContext;