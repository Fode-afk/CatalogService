using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductWithdrawPublishSubmissionContext(bool VendorIsActive) : IVendorContext;