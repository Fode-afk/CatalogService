using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductInfoUpdatedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductInfoUpdatedDomainEvent>
{
    public async Task Handle(ProductInfoUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(x => x.Id == notification.ProductId, cancellationToken);

        if (productCardReadModel == null)
            return;

        productCardReadModel.Name = notification.Name;
        productCardReadModel.NameNormalized = Name.Normalize(notification.Name);
        productCardReadModel.Description = notification.Description;
        productCardReadModel.ShortDescription = notification.ShortDescription;
        productCardReadModel.CategoryId = notification.CategoryId;
        productCardReadModel.BrandId = Guid.Empty;

        productCardReadModel.SeoTitle = notification.SeoMetadata.Title.Value;
        productCardReadModel.SeoDescription = notification.SeoMetadata.Description.Value;
        productCardReadModel.SeoKeywords = notification.SeoMetadata.Keywords.Value;

        productCardReadModel.UpdatedAt = notification.UpdatedAt;
    }
}