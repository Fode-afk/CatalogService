using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardCreatedDomainEvent(
    Guid ProductCardId,
    Name Name,
    Slug Slug,
    Description Description,
    ShortDescription ShortDescription,
    Guid CategoryId,
    Guid VendorId,
    Brand Brand,
    ProductCardStatus ProductCardStatus,
    SeoMetadata SeoMetadata,
    List<ProductCardAttribute> Attributes,
    List<Tag> Tags,
    DateTimeOffset CreatedAt) : IDomainEvent;