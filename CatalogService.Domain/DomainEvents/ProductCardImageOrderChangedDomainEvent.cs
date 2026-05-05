using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardImageOrderChangedDomainEvent(
    Guid ProductCardId,
    IReadOnlyList<ProductCardImage> Images,
    DateTimeOffset UpdatedAt) : IDomainEvent;