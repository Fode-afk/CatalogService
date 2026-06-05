using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed record DeleteVendorSnapshotCommand(Guid VendorId) : IRequest;