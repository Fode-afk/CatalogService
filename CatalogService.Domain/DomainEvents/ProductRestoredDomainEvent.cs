using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductRestoredDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    long Version) : IDomainEvent;