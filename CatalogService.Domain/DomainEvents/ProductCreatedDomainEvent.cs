using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    Guid BrandId,
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    SeoMetadata SeoMetadata,
    ProductStatus ProductStatus,
    bool CanBeModified,
    long Version) : IDomainEvent;