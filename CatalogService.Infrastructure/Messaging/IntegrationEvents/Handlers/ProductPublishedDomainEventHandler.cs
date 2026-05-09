using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductPublishedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductPublishedDomainEvent>
{
    public async Task Handle(ProductPublishedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductPublishedIntegrationEvent(
            notification.ProductId,
            notification.VendorId,
            notification.ProductStatus), cancellationToken);
}