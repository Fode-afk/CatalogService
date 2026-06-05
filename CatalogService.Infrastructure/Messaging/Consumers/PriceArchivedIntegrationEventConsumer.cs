using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Pricing;

namespace CatalogService.Infrastructure.Messaging.Consumers;

public sealed class PriceArchivedIntegrationEventConsumer(IMediator mediator) : IConsumer<PriceArchivedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PriceArchivedIntegrationEvent> context) =>
        await mediator.Send(new DeleteProductVariantPriceSnapshotCommand(context.Message.ProductVariantId), context.CancellationToken);
}
