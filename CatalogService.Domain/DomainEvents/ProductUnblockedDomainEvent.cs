using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Primitives;
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