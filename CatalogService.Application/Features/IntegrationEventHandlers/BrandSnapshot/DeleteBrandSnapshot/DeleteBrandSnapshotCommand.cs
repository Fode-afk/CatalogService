using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;

public sealed record DeleteBrandSnapshotCommand(Guid BrandId) : IRequest;