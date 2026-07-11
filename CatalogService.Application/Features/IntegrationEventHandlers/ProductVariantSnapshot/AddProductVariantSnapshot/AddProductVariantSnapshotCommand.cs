using MediatR;
using migApp.Shared.Dtos.ProductVariant;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;

public sealed record AddProductVariantSnapshotCommand(
    Guid ProductVariantId,
    Guid ProductId,
    bool HasMainImage,
    long Version,
    List<VariantAttributeDto> CharacteristicValues) : IRequest;
