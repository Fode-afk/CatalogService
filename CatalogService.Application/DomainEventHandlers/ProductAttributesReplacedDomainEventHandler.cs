using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductAttributesReplacedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductAttributesReplacedDomainEvent>
{
    public async Task Handle(ProductAttributesReplacedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

        if (productReadModel == null)
            return;

        var attributesDict = notification.Attributes
            .ToDictionary(a => a.Name.Value, a => a.Value.Value);

        productReadModel.AttributesJson = JsonSerializer.Serialize(attributesDict);
        productReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
