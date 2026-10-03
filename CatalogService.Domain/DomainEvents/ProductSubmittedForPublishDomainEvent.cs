using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.DomainEvents;

public sealed record ProductSubmittedForPublishDomainEvent(
    Guid ProductId,
    Guid SubmissionId,
    Guid CategoryId,
    Guid VendorId,
    bool CanBeModified,
    ProductStatus ProductStatus,
    List<ProductSuspensionReason> SuspensionReasons,
    RejectionReason? RejectionReason,
    DateTimeOffset? SubmittedForPublishApprovalAt,
    bool IsVisiblePublicly,
    long Version) : IDomainEvent;
