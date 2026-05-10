using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.UpdateCategorySnapshot;

public sealed record UpdateCategorySnapshotCommand(
    Guid CategoryId,
    bool IsActive,
    long Version) : IRequest<IResult>;