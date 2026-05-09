using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Products;
namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductCreatedDomainEvent>
{
    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(
            new ProductCreatedIntegrationEvent(
                notification.ProductId,
                notification.VendorId,
                notification.CategoryId,
                notification.CanBeModified), cancellationToken);
}