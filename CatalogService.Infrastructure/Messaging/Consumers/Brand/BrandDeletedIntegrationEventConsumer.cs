using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;
using MassTransit;
using MediatR;
using migApp.Shared.Messaging.IntegrationEvents.Brands;

namespace CatalogService.Infrastructure.Messaging.Consumers.Brand;

public sealed class BrandDeletedIntegrationEventConsumer(ISender sender) : IConsumer<BrandDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BrandDeletedIntegrationEvent> context) =>
        await sender.Send(new DeleteBrandSnapshotCommand(context.Message.BrandId), context.CancellationToken);
}
