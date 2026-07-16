using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductRestoredDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductRestoredDomainEvent>
{
    public async Task Handle(ProductRestoredDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductRestoredIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.VendorId,
            notification.CanBeModified,
            notification.ProductStatus,
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}
