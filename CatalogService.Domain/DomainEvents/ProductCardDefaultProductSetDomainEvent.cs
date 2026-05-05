using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardDefaultProductSetDomainEvent(
    Guid ProductCardId,
    Guid NewDefaultProductId,
    Guid? OldDefaultProductId) : IDomainEvent;