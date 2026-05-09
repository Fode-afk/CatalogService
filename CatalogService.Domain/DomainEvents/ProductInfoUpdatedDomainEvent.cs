using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductInfoUpdatedDomainEvent(
    Guid ProductId,
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    Guid CategoryId,
    Guid BrandId,
    SeoMetadata SeoMetadata,
    DateTimeOffset UpdatedAt) : IDomainEvent;