using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductSuspendedDomainEvent(
    Guid ProductId,
    ProductStatus ProductStatus,
    List<ProductSuspensionReason> SuspensionReasons,
    long Version) : IDomainEvent;