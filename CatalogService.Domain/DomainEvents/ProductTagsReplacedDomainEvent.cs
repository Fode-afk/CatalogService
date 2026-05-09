using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductTagsReplacedDomainEvent(
    Guid ProductId,
    IReadOnlyList<Tag> Tags,
    DateTimeOffset UpdatedAt) : IDomainEvent;