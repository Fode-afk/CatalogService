using CatalogService.Domain.DomainEvents;
using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductMarketsChangedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductMarketsChangedDomainEvent>
{
    public async Task Handle(ProductMarketsChangedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductMarketsChangedIntegrationEvent(
            notification.ProductId,
            [..notification.Markets.Select(m => m.Code)],
            [..notification.AddedMarkets.Select(m => m.Code)],
            [..notification.RemovedMarkets.Select(m => m.Code)],
            notification.Version), cancellationToken);
}
