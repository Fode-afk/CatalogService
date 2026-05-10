using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductLockedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductLockedDomainEvent>
{
    public async Task Handle(ProductLockedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductLockedIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.CanBeModified,
            notification.Version), cancellationToken);
}
