using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductCardDefaultProductSetDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCardDefaultProductSetDomainEvent>
{
    public async Task Handle(ProductCardDefaultProductSetDomainEvent notification, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductCardReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductCardId, cancellationToken);

        if (productCardReadModel == null)
            return;

        productCardReadModel.DefaultProductId = notification.NewDefaultProductId;
        productCardReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
