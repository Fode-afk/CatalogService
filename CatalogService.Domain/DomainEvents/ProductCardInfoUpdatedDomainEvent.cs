using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardInfoUpdatedDomainEvent(Guid ProductCardId) : IDomainEvent;