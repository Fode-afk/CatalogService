using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnpublishedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    bool CanEditContent,
    bool CanEditOperationalData,
    ProductStatus ProductStatus,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;