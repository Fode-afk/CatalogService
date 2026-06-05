using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;

public sealed record AddProductVariantSnapshotCommand(
    Guid ProductVariantId,
    Guid ProductId,
    bool HasMainImage,
    long Version,
    List<ProductVariantCharacteristicValue> CharacteristicValues) : IRequest;
