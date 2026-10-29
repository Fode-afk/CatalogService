using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    Guid BrandId,
    ProductName Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    SeoMetadata SeoMetadata,
    ProductStatus ProductStatus,
    bool CanEditContent,
    bool CanEditOperationalData,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;