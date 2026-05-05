using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductCardAttributesReplacedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCardAttributesReplacedDomainEvent>
{
    public async Task Handle(ProductCardAttributesReplacedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productReadModel = await context.ProductCardReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductCardId, cancellationToken);

        if (productReadModel == null)
            return;

        var attributesDict = notification.Attributes
            .ToDictionary(a => a.Name.Value, a => a.Value.Value);

        productReadModel.AttributesJson = JsonSerializer.Serialize(attributesDict);
        productReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
