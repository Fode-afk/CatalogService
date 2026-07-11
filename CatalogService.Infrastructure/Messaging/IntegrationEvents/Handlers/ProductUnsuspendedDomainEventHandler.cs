using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductUnsuspendedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductUnsuspendedDomainEvent>
{
    public async Task Handle(ProductUnsuspendedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductUnsuspendedIntegrationEvent(
            notification.ProductId,
            notification.ProductStatus,
            [..notification.SuspensionReasons.Select(r => r.Reason)],
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}
