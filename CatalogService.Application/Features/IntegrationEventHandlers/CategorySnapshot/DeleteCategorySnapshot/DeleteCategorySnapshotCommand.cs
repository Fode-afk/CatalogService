using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.DeleteCategorySnapshot;

public sealed record DeleteCategorySnapshotCommand(Guid CategoryId) : IRequest;