using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardAttributesReplacedDomainEvent(
    Guid ProductCardId,
    IReadOnlyList<ProductCardAttribute> Attributes,
    DateTimeOffset UpdatedAt) : IDomainEvent;