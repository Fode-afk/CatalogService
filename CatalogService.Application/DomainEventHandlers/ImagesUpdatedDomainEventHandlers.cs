using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Models;
using CatalogService.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CatalogService.Application.DomainEventHandlers;

public sealed class ImagesUpdatedDomainEventHandlers(IAppDbContext context) : 
    IPreCommitDomainEventHandler<ProductCardImageAddedDomainEvent>,
    IPreCommitDomainEventHandler<ProductCardImageAltUpdatedDomainEvent>,
    IPreCommitDomainEventHandler<ProductCardImageOrderChangedDomainEvent>,
    IPreCommitDomainEventHandler<ProductCardImageRemovedDomainEvent>,
    IPreCommitDomainEventHandler<ProductCardImageSetMainDomainEvent>
{
    public async Task Handle(ProductCardImageAddedDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductCardId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductCardImageAltUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
         await UpdateImages(
            notification.ProductCardId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductCardImageOrderChangedDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductCardId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductCardImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductCardId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    public async Task Handle(ProductCardImageSetMainDomainEvent notification, CancellationToken cancellationToken) =>
        await UpdateImages(
            notification.ProductCardId,
            notification.Images,
            notification.UpdatedAt,
            cancellationToken);

    private async Task UpdateImages(
        Guid productCardId,
        IReadOnlyList<ProductCardImage> images,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken = default)
    {
        var productCardReadModel = await context.ProductCardReadModels
            .FirstOrDefaultAsync(x => x.Id == productCardId, cancellationToken);

        if (productCardReadModel == null)
            return;

        var imagesConeverted = images
           .OrderBy(i => i.SortOrder)
           .Select(i => new
           {
               Url = i.Url.Value,
               Alt = i.Alt.Value,
               i.IsMain,
               i.SortOrder
           })
           .ToList();

        var mainImage = images
            .FirstOrDefault(i => i.IsMain)?.Url.Value;

        productCardReadModel.ImagesJson = JsonSerializer.Serialize(imagesConeverted);
        productCardReadModel.MainImage = mainImage ?? string.Empty;
        productCardReadModel.UpdatedAt = updatedAt;
    }
}
