using CatalogService.Application.Features.Commands.AddProductVariantSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;

namespace CatalogService.Infrastructure.Messaging.Consumers;

public sealed class ProductVariantCreatedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductVariantCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductVariantCreatedIntegrationEvent> context) =>
        await mediator.Send(new AddProductVariantSnapshotCommand(
            context.Message.ProductVariantId,
            context.Message.ProductId,
            context.Message.HasMainImage,
            context.Message.CharacteristicValues), context.CancellationToken);
}