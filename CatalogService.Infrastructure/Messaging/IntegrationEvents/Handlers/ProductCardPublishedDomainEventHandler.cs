using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductCards;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductCardPublishedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductCardPublishedDomainEvent>
{
    public async Task Handle(ProductCardPublishedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductCardPublishedIntegrationEvent(
            notification.ProductCardId,
            notification.VendorId,
            notification.ProductCardStatus), cancellationToken);
}