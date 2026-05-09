using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductVariantAddedDomainEvent(Guid ProductId) : IDomainEvent;