using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;

public sealed record DeleteProductVariantSnapshotCommand(Guid ProductVariantId) : IRequest<IResult>;