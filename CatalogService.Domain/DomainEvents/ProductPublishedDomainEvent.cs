using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductPublishedDomainEvent(
    Guid ProductId,
    Guid VendorId,
    ProductStatus ProductStatus,
    DateTimeOffset UpdatedAt) : IDomainEvent;