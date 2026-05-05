using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.ProductCards;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductCardArchivedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductCardArchivedDomainEvent>
{
    public async Task Handle(ProductCardArchivedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductCardArchivedIntegrationEvent(
            notification.ProductCardId,
            notification.VendorId,
            notification.ProductCardStatus), cancellationToken);
}
