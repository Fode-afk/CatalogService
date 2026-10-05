using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductPublishRejectedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    bool CanEditContent,
    bool CanEditOperationalData,
    ProductStatus ProductStatus,
    RejectionReason? RejectionReason,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;