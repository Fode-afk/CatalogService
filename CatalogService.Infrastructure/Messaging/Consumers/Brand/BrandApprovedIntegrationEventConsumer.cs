using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.UpdateBrandSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Brands;

namespace CatalogService.Infrastructure.Messaging.Consumers.Brand;

public sealed class BrandApprovedIntegrationEventConsumer(ISender sender) : IConsumer<BrandApprovedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BrandApprovedIntegrationEvent> context) =>
        await sender.Send(new UpdateBrandSnapshotCommand(
            context.Message.BrandId,
            context.Message.IsAssignable,
            context.Message.Version), context.CancellationToken);
}
