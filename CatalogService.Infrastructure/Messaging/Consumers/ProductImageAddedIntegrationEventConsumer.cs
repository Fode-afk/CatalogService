using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;

namespace CatalogService.Infrastructure.Messaging.Consumers;

public sealed class ProductImageAddedIntegrationEventConsumer(IMediator mediator) : IConsumer<ProductImageAddedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductImageAddedIntegrationEvent> context) => 
        await mediator.Send(new UpdateProductVariantSnapshotCommand(
            context.Message.ProductVariantId,
            context.Message.HasMainImage,
            context.Message.Version), context.CancellationToken);
}