using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.AddBrandSnapshot;

public sealed record AddBrandSnapshotCommand(
    Guid BrandId, 
    bool IsActive,
    long Version) : IRequest<IResult>;