using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;

public sealed record DeleteBrandSnapshotCommand(Guid BrandId) : IRequest<IResult>;