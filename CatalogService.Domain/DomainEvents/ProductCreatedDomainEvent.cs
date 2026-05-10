using CatalogService.Domain.Primitives;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    Guid BrandId,
    bool CanBeModified,
    long Version) : IDomainEvent;