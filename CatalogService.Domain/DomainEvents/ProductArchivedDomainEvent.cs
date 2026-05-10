using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductArchivedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    long Version) : IDomainEvent;