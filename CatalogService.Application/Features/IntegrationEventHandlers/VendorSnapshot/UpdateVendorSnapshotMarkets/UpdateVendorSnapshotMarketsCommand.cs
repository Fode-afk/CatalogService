using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorMarkets;

public sealed record UpdateVendorSnapshotMarketsCommand(
    Guid VendorId,
    List<string> Markets,
    long Version) : IRequest;