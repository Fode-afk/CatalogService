using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductAttributesReplacedDomainEvent(
    Guid ProductId,
    IReadOnlyList<ProductAttribute> Attributes,
    DateTimeOffset UpdatedAt) : IDomainEvent;