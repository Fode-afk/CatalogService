using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnlockedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    long Version) : IDomainEvent;