using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnlockedDomainEvent(
    Guid ProductId,
    bool IsLockedByAdmin,
    DateTimeOffset UpdatedAt) : IDomainEvent;