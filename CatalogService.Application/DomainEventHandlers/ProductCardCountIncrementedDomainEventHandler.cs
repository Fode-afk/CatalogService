using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductCardCountIncrementedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCardCountIncrementedDomainEvent>
{
    public async Task Handle(ProductCardCountIncrementedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productReadModel = await context.ProductCardReadModels
           .FirstOrDefaultAsync(x => x.Id == notification.ProductCardId, cancellationToken);

        if (productReadModel == null)
            return;

        productReadModel.ProductCount = notification.ProductCount;
        productReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
