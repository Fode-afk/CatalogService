using MediatR;
using migApp.Shared.Results;
namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;

public sealed record DeleteProductVariantPriceSnapshotCommand(Guid ProductVariantId) : IRequest<IResult>;