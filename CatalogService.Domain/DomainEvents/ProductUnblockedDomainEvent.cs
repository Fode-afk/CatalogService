using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnblockedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    bool CanEditContent,
    bool CanEditOperationalData,
    bool IsBlocked,
    ProductStatus ProductStatus,
    BlockReason? BlockReason,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;