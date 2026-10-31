using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.ProductVariants;

namespace CatalogService.Infrastructure.Messaging.Consumers.ProductVariant;

public sealed class ProductVariantImageAddedIntegrationEventConsumer(ISender sender) : IConsumer<ProductVariantImageAddedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductVariantImageAddedIntegrationEvent> context) => 
        await sender.Send(new UpdateProductVariantSnapshotCommand(
            context.Message.ProductVariantId,
            context.Message.HasMainImage,
            context.Message.Version), context.CancellationToken);
}