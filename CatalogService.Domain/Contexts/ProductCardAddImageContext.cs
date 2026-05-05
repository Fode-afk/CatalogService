using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardAddImageContext(
    bool VendorIsActive,
    ProductCardStatus ProductCardStatus,
    int ImagesCount) : IVendorContext, IStatusContext;