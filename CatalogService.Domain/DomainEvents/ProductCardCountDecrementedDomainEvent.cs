using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardCountDecrementedDomainEvent(
    Guid ProductCardId,
    ProductCount ProductCount,
    DateTimeOffset UpdatedAt) : IDomainEvent;