using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardTagsReplacedDomainEvent(
    Guid ProductCardId,
    IReadOnlyList<Tag> Tags,
    DateTimeOffset UpdatedAt) : IDomainEvent;