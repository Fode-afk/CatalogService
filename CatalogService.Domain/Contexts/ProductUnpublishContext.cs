using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Contexts;

public sealed record ProductUnpublishContext(
    bool VendorIsActive,
    ProductStatus ProductStatus) : IVendorContext;