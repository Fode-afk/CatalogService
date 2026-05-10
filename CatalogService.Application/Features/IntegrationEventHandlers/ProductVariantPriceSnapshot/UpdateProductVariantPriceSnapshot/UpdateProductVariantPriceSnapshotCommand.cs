using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.UpdateProductVariantPriceSnapshot;

public sealed record UpdateProductVariantPriceSnapshotCommand(
    Guid ProductVariantId,
    bool HasPrice,
    long Version) : IRequest<IResult>;