using CatalogService.Domain.Primitives;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductUnpublishedDomainEvent(
    Guid ProductId,
    Guid CategoryId,
    bool CanBeModified,
    ProductStatus ProductStatus,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;