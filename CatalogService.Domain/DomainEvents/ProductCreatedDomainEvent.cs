using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    Guid CategoryId,
    Guid VendorId,
    Guid BrandId,
    ProductStatus ProductStatus,
    SeoMetadata SeoMetadata,
    bool CanBeModified,
    DateTimeOffset CreatedAt) : IDomainEvent;