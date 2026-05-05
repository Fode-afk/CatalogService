using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCountDecrementedContext(
    bool VendorIsActive,
    ProductCardStatus ProductCardStatus) : IVendorContext, IStatusContext;