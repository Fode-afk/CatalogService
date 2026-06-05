using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;

public sealed record UpdateProductVariantSnapshotCommand(
    Guid ProductVariantId,
    bool HasMainImage,
    long Version) : IRequest;