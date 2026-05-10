using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.UpdateBrandSnapshot;

public sealed record UpdateBrandSnapshotCommand(
    Guid BrandId,
    bool IsActive,
    int Version) : IRequest<IResult>;