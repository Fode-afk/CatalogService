using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductArchivedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductArchivedDomainEvent>
{
    public async Task Handle(ProductArchivedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductArchivedIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.VendorId,
            notification.CanBeModified,
            notification.ProductStatus,
            [..notification.SuspensionReasons.Select(r => r.Reason)],
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}
