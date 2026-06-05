using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;

public sealed record AddProductVariantPriceSnapshotCommand(
    Guid ProductVariantId, 
    bool HasPrice,
    long Version) : IRequest;