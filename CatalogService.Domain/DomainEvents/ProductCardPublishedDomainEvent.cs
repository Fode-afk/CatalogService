using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardPublishedDomainEvent(Guid ProductCardId) : IDomainEvent;