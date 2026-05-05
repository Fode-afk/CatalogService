using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductCards;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductCardDefaultProductSetDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductCardDefaultProductSetDomainEvent>
{
    public async Task Handle(ProductCardDefaultProductSetDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductCardDefaultProductSetIntegrationEvent(
            notification.ProductCardId,
            notification.NewDefaultProductId,
            notification.OldDefaultProductId), cancellationToken);
}
