using CatalogService.Domain.DomainEvents;
using MassTransit;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Dtos.Product;
using migApp.Shared.Messaging.IntegrationEvents.Products;

namespace CatalogService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class ProductAttributesReplacedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<ProductAttributesReplacedDomainEvent>
{
    public async Task Handle(ProductAttributesReplacedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new ProductAttributesReplacedIntegrationEvent(
            notification.ProductId,
            [.. notification.Attributes.Select(a => new ProductAttributeDto(
                a.CharacteristicId,
                a.Name,
                a.Value?.Value,
                a.CharType,
                a.GroupName?.Value,
                a.IsVariable,
                a.IsUnifying,
                [.. a.VariableValues.Select(v => new AttributeVariableValueDto(
                    v.Value,
                    v.ValueId))]))],
            notification.Version), cancellationToken);
}
