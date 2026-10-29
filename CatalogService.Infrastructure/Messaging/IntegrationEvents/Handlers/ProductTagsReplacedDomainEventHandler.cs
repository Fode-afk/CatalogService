using CatalogService.Domain.DomainEvents;
using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductCardTagsReplacedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductTagsReplacedDomainEvent>
{
    public async Task Handle(ProductTagsReplacedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductTagsReplacedIntegrationEvent(
            notification.ProductId,
            [.. notification.Tags.Select(t => t.Value)],
            notification.Version), cancellationToken);
}
