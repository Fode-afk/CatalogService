using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductArchivedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    ProductStatus ProductStatus,
    List<ProductSuspensionReason> SuspensionReasons,
    long Version) : IDomainEvent;