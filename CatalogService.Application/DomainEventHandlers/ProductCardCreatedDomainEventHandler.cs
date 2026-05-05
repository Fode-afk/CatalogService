using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using CatalogService.Domain.ValueObjects;
using System.Text.Json;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ProductCardCreatedDomainEventHandler(IAppDbContext context) : IPreCommitDomainEventHandler<ProductCardCreatedDomainEvent>
{
    public Task Handle(ProductCardCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var attributesDict = notification.Attributes
            .ToDictionary(a => a.Name.Value, a => a.Value.Value);

        var tags = notification.Tags
            .Select(t => t.Value)
            .ToList();

        var productCardReadModel = new ProductCardReadModel
        {
            Id = notification.ProductCardId,
            Name = notification.Name.Value,
            NameNormalized = Name.Normalize(notification.Name),
            Slug = notification.Slug.Value,
            Description = notification.Description.Value,
            ShortDescription = notification.ShortDescription.Value,

            CategoryId = notification.CategoryId,
            CategoryName = notification.Name,
            CategorySlug = notification.Slug,

            VendorId = notification.VendorId,
            Brand = notification.Brand,

            CreatedAt = notification.CreatedAt,

            SeoTitle = notification.SeoMetadata.Title.Value,
            SeoDescription = notification.SeoMetadata.Description.Value,
            SeoKeywords = notification.SeoMetadata.Keywords.Value,

            AttributesJson = JsonSerializer.Serialize(attributesDict),
            TagsJson = JsonSerializer.Serialize(tags),
            TagsFlat = string.Join(",", tags)
        };

        context.ProductCardReadModels.Add(productCardReadModel);

        return Task.CompletedTask;
    }
}
