using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.ProductCards;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCardArchivedDomainEvent(
    Guid ProductCardId,
    Guid VendorId,
    ProductCardStatus ProductCardStatus,
    DateTimeOffset UpdatedAt) : IDomainEvent;