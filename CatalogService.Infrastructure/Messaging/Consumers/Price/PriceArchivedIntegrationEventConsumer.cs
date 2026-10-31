using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;

namespace CatalogService.Infrastructure.Messaging.Consumers.Price;

public sealed class PriceArchivedIntegrationEventConsumer(ISender sender) : IConsumer<PriceArchivedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PriceArchivedIntegrationEvent> context) =>
        await sender.Send(new DeleteProductVariantPriceSnapshotCommand(context.Message.PriceId), context.CancellationToken);
}
