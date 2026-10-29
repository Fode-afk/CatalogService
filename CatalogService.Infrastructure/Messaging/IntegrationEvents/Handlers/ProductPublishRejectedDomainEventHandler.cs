using CatalogService.Domain.DomainEvents;
using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductPublishRejectedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductPublishRejectedDomainEvent>
{
    public async Task Handle(ProductPublishRejectedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductPublishRejectedIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.VendorId,
            notification.CanEditContent,
            notification.CanEditOperationalData,
            notification.ProductStatus,
            notification.RejectionReason?.ToString(),
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}