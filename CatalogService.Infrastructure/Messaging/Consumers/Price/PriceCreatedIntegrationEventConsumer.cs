using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;

namespace CatalogService.Infrastructure.Messaging.Consumers.Price;

public sealed class PriceCreatedIntegrationEventConsumer(ISender sender) : IConsumer<PriceCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PriceCreatedIntegrationEvent> context)
    {
        if (context.Message.IsBase)
            await sender.Send(new AddProductVariantPriceSnapshotCommand(
                context.Message.PriceId, 
                context.Message.ProductVariantId), context.CancellationToken);
    }
}