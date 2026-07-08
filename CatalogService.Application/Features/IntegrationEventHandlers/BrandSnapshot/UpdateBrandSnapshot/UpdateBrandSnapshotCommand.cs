using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.UpdateBrandSnapshot;

public sealed record UpdateBrandSnapshotCommand(
    Guid BrandId,
    bool IsActive,
    long Version) : IRequest;