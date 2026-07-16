using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductSuspendedDomainEvent(
    Guid ProductId,
    Guid VendorId,
    ProductStatus ProductStatus,
    List<ProductSuspensionReason> SuspensionReasons,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;