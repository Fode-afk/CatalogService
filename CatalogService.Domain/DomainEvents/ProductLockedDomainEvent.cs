using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductLockedDomainEvent(
    Guid ProductId,
    bool IsLockedByAdmin,
    DateTimeOffset UpdatedAt) : IDomainEvent;