using CatalogService.Domain.DomainEvents;
using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductUnpublishedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductUnpublishedDomainEvent>
{
    public async Task Handle(ProductUnpublishedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductUnpublishedIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.VendorId,
            notification.CanEditContent,
            notification.CanEditOperationalData,
            notification.ProductStatus,
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}
