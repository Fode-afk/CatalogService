using CatalogService.Domain.Abstractions;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.Contexts;

public sealed record ProductCardTagsReplacedContext(
    bool VendorIsActive,
    ProductCardStatus ProductCardStatus,
    IReadOnlyCollection<Tag> Tags) :
        IVendorContext,
        IStatusContext,
        ITagsContext;
