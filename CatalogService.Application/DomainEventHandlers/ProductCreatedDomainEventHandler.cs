using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using System.Text.Json;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductCreatedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCreatedDomainEvent>
{
    public Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //var productCardReadModel = new ProductReadModel
        //{
        //    Id = notification.ProductId,
        //    Name = notification.Name.Value,
        //    NameNormalized = Name.Normalize(notification.Name),
        //    Slug = notification.Slug.Value,
        //    Description = notification.Description.Value,
        //    ShortDescription = notification.ShortDescription.Value,
        //
        //    CategoryId = notification.CategoryId,
        //    CategoryName = notification.Name,
        //    CategorySlug = notification.Slug,
        //
        //    VendorId = notification.VendorId,
        //    VendorName = notification.Name,
        //
        //    BrandId = Guid.Empty,
        //    BrandName = notification.Name,
        //
        //    CreatedAt = notification.CreatedAt,
        //
        //    SeoTitle = notification.SeoMetadata.Title.Value,
        //    SeoDescription = notification.SeoMetadata.Description.Value,
        //    SeoKeywords = notification.SeoMetadata.Keywords.Value,
        //
        //    AttributesJson = "",
        //    TagsJson = "",
        //    TagsFlat = ""
        //};
        //
        //context.ProductReadModels.Add(productCardReadModel);

        return Task.CompletedTask;
    }
}
