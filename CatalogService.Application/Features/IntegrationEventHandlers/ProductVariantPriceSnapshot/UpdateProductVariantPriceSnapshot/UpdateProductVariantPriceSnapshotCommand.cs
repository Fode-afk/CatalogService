using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.UpdateProductVariantPriceSnapshot;

public sealed record UpdateProductVariantPriceSnapshotCommand(
    Guid ProductVariantId,
    bool HasPrice,
    long Version) : IRequest;