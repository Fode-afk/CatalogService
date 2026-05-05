using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardCountIncrementedDomainEvent(
    Guid ProductCardId,
    ProductCount ProductCount,
    DateTimeOffset UpdatedAt) : IDomainEvent;