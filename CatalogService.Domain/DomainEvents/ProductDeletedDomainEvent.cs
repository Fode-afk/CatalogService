using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductDeletedDomainEvent(Guid ProductId) : IDomainEvent;