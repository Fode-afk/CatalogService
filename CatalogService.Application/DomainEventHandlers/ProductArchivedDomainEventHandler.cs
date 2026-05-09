using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductArchivedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductArchivedDomainEvent>
{
    public async Task Handle(ProductArchivedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

        if (productCardReadModel == null)
            return;

        productCardReadModel.ProductStatus = notification.ProductStatus;
        productCardReadModel.UpdatedAt = notification.UpdatedAt;
    }
}