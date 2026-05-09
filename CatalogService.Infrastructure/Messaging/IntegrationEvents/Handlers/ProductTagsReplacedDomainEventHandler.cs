using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

//public sealed class ProductCardTagsReplacedDomainEventHandler(IPublishEndpoint publish) : //IPreCommitDomainEventHandler<ProductCardTagsReplacedDomainEvent>
//{
//    public async Task Handle(ProductCardTagsReplacedDomainEvent notification, CancellationToken cancellationToken) =>
//        await publish.Publish(new ProductCardTagsReplacedIntegrationEvent(), cancellationToken);
//}
