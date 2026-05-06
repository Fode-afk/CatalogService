using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardPublishContext(
    bool VendorIsActive,
    bool HasDefaultProduct,
    bool DefaultProductHasPrice,
    bool DefaultProductInStock,
    ProductCardStatus ProductCardStatus,
    int ImagesCount) : 
        IVendorContext,
        IStatusContext;