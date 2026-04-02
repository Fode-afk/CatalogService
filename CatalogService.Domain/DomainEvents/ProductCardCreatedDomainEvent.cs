using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardCreatedDomainEvent(Guid ProductCardId) : IDomainEvent;