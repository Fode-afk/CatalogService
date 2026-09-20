using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;

public sealed record AddVendorSnapshotCommand(
    Guid VendorId,
    bool IsActive,
    long Version) : IRequest;