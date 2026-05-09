using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductVariantRemovedDomainEvent(Guid ProductId) : IDomainEvent;