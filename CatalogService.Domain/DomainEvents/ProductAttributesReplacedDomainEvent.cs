using CatalogService.Domain.Models;
using migApp.Shared.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductAttributesReplacedDomainEvent(
    Guid ProductId,
    IReadOnlyList<ProductAttribute> Attributes,
    long Version) : IDomainEvent;