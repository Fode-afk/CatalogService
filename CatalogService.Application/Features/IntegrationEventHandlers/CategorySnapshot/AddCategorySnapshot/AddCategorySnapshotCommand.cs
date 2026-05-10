using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.AddCategorySnapshot;

public sealed record AddCategorySnapshotCommand(
    Guid CategoryId,
    bool IsActive, 
    long Version) : IRequest<IResult>;