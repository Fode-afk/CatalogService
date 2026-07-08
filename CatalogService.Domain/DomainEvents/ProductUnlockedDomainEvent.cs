using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnlockedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    bool IsLockedByAdmin,
    ProductStatus ProductStatus,
    long Version) : IDomainEvent;