using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using MassTransit;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

//public sealed class ProductCardInfoUpdatedDomainEventHandler(IPublishEndpoint publish) : //IPreCommitDomainEventHandler<ProductCardInfoUpdatedDomainEvent>
//{
//    public async Task Handle(ProductCardInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
//        await publish.Publish(new ProductCardInfoUpdatedIntegrationEvent(), cancellationToken);
//}
