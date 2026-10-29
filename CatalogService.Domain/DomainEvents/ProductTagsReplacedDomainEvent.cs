using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductTagsReplacedDomainEvent(
    Guid ProductId,
    IReadOnlyList<Tag> Tags,
    long Version) : IDomainEvent;