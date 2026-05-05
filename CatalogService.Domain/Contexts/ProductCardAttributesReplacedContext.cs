using CatalogService.Domain.Abstractions;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardAttributesReplacedContext(
    bool VendorIsActive,
    ProductCardStatus ProductCardStatus,
    IReadOnlyCollection<ProductCardAttribute> Attributes) :
        IVendorContext,
        IStatusContext,
        IAttributesContext;