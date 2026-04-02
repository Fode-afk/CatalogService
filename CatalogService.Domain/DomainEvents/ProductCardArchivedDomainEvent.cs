using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardArchivedDomainEvent(Guid ProductCardId) : IDomainEvent;