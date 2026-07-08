using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductInfoUpdatedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid BrandId,
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    SeoMetadata SeoMetadata,
    bool CanBeModified,
    long Version) : IDomainEvent;