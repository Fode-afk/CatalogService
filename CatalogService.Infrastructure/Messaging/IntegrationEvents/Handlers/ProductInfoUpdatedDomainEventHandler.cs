using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductInfoUpdatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductInfoUpdatedDomainEvent>
{
    public async Task Handle(ProductInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductInfoUpdatedIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.CanBeModified,
            notification.Version), cancellationToken);
}
