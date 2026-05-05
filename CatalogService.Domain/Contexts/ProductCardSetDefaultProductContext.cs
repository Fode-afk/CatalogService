using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardSetDefaultProductContext(
    bool VendorIsActive,
    ProductCardStatus ProductCardStatus,
    bool DefaultProductBelongsToCard) : IVendorContext, IStatusContext;