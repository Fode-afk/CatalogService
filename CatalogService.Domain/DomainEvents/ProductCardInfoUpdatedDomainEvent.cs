using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardInfoUpdatedDomainEvent(
    Guid ProductCardId,
    Name Name,
    Description Description,
    ShortDescription ShortDescription,
    Guid CategoryId,
    Brand Brand,
    SeoMetadata SeoMetadata,
    DateTimeOffset UpdatedAt) : IDomainEvent;