using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed record UpdateVendorSnapshotCommand(
    Guid VendorId,
    bool IsActive,
    long Version) : IRequest;