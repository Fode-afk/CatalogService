using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductVariantAddedDomainEvent(
    Guid ProductId,
    IReadOnlyList<ProductAttribute> Attributes,
    long Version) : IDomainEvent;