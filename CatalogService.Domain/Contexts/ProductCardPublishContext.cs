using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardPublishContext(
    bool VendorIsActive,
    bool HasDefaultProduct,
    bool DefaultProductHasPrice,
    bool DefaultProductInStock,
    ProductCardStatus ProductCardStatus,
    ProductCount ProductCount,
    IReadOnlyCollection<ProductCardImage> Images) : 
        IVendorContext,
        IStatusContext;