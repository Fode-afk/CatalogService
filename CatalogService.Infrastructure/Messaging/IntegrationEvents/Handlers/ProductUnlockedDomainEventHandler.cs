using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductUnlockedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductUnlockedDomainEvent>
{
    public async Task Handle(ProductUnlockedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductUnlockedIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.CanBeModified,
            notification.Version), cancellationToken);
}
