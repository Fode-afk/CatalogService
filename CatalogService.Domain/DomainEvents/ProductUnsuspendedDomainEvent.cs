using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnsuspendedDomainEvent(
    Guid ProductId,
    ProductStatus ProductStatus,
    DateTimeOffset UpdatedAt) : IDomainEvent;