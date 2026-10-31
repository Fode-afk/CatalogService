using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.UpdateBrandSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Brands;

namespace CatalogService.Infrastructure.Messaging.Consumers.Brand;

public sealed class BrandRejectedIntegrationEventConsumer(ISender sender) : IConsumer<BrandRejectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BrandRejectedIntegrationEvent> context) =>
        await sender.Send(new UpdateBrandSnapshotCommand(
            context.Message.BrandId,
            context.Message.IsAssignable,
            context.Message.Version), context.CancellationToken);
}