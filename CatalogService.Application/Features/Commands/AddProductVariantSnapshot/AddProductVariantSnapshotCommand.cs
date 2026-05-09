using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.AddProductVariantSnapshot;

public sealed record AddProductVariantSnapshotCommand(
    Guid ProductVariantId,
    Guid ProductId,
    bool HasMainImage,
    List<ProductVariantCharacteristicValue> CharacteristicValues) : IRequest<IResult>;
