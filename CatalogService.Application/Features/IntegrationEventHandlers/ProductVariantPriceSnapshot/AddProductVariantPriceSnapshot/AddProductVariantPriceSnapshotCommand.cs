using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;

public sealed record AddProductVariantPriceSnapshotCommand(
    Guid ProductVariantId, 
    Guid ProductId,
    bool HasPrice,
    long Version) : IRequest;