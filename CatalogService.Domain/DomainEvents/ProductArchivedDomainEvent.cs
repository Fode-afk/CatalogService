using CatalogService.Domain.Models;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductArchivedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    Guid VendorId,
    bool CanEditContent,
    bool CanEditOperationalData,
    ProductStatus ProductStatus,
    List<ProductSuspensionReason> SuspensionReasons,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;