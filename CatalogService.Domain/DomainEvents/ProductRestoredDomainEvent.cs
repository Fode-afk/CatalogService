using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductRestoredDomainEvent(
    Guid ProductId,
    ProductStatus ProductStatus,
    DateTimeOffset UpdatedAt) : IDomainEvent;