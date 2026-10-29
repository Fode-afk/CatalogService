using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductInfoUpdatedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid BrandId,
    ProductName Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    SeoMetadata SeoMetadata,
    bool CanEditContent,
    bool CanEditOperationalData,
    long Version) : IDomainEvent;