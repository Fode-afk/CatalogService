using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductCardPublishedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCardPublishedDomainEvent>
{
    public async Task Handle(ProductCardPublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductCardReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductCardId, cancellationToken);

        if (productCardReadModel == null)
            return;

        productCardReadModel.ProductCardStatus = notification.ProductCardStatus;
        productCardReadModel.UpdatedAt = notification.UpdatedAt;
    }
}
