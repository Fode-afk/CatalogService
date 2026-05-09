using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductRestoreContext(bool VendorIsActive) : IVendorContext;