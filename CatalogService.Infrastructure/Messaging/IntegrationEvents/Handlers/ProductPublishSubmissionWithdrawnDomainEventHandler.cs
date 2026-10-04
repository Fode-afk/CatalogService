using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductPublishSubmissionWithdrawnDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductPublishSubmissionWithdrawnDomainEvent>
{
    public async Task Handle(ProductPublishSubmissionWithdrawnDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductPublishSubmissionWithdrawnIntegrationEvent(
            notification.ProductId,
            notification.CategoryId,
            notification.VendorId,
            notification.CanBeModified,
            notification.ProductStatus,
            notification.IsVisiblePublicly,
            notification.Version), cancellationToken);
}