using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using ZiggyCreatures.Caching.Fusion;

namespace CatalogService.Application.Caching;

public sealed class InvalidateProductCardCacheHandlers(IFusionCache cache) : 
    IPostCommitDomainEventHandler<ProductCardInfoUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardPublishedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardArchivedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardAttributesReplacedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardTagsReplacedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardCountIncrementedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardCountDecrementedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardImageAddedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardImageRemovedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardImageOrderChangedDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardImageSetMainDomainEvent>,
    IPostCommitDomainEventHandler<ProductCardImageAltUpdatedDomainEvent>
{
    public async Task Handle(ProductCardInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardPublishedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardArchivedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardAttributesReplacedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardTagsReplacedDomainEvent notification, CancellationToken cancellationToken) =>
         await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardCountIncrementedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardCountDecrementedDomainEvent notification, CancellationToken cancellationToken) =>
         await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardImageAddedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardImageRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardImageOrderChangedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardImageSetMainDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    public async Task Handle(ProductCardImageAltUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductCardId, cancellationToken);

    private async Task HandleInternal(Guid id, CancellationToken cancellationToken) =>
      await cache.RemoveByTagAsync(CacheTags.ProductCardById(id), token: cancellationToken);
}
