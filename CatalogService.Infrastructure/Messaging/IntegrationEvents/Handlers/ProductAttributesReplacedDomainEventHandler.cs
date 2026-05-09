using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

//public sealed class ProductAttributesReplacedDomainEventHandler(IPublishEndpoint publish) : //IPreCommitDomainEventHandler<ProductCardAttributesReplacedDomainEvent>
//{
//    public async Task Handle(ProductCardAttributesReplacedDomainEvent notification, CancellationToken //cancellationToken) =>
//        await publish.Publish(new ProductCardAttributesReplacedIntegrationEvent(), cancellationToken);
//}
