using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductSuspendedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductSuspendedDomainEvent>
{
    public async Task Handle(ProductSuspendedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductSuspendedIntegrationEvent(
            notification.ProductId,
            notification.VendorId,
            notification.ProductStatus,
            [..notification.SuspensionReasons.Select(r => r.Reason)],
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}
