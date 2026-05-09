using CatalogService.Domain.DomainEvents;
using CatalogService.Domain.Primitives;
using ZiggyCreatures.Caching.Fusion;

namespace CatalogService.Application.Caching;

public sealed class InvalidateProductCardCacheHandlers(IFusionCache cache) : 
    IPostCommitDomainEventHandler<ProductInfoUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<ProductPublishedDomainEvent>,
    IPostCommitDomainEventHandler<ProductArchivedDomainEvent>,
    IPostCommitDomainEventHandler<ProductAttributesReplacedDomainEvent>,
    IPostCommitDomainEventHandler<ProductTagsReplacedDomainEvent>
{
    public async Task Handle(ProductInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductId, cancellationToken);

    public async Task Handle(ProductPublishedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductId, cancellationToken);

    public async Task Handle(ProductArchivedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductId, cancellationToken);

    public async Task Handle(ProductAttributesReplacedDomainEvent notification, CancellationToken cancellationToken) =>
        await HandleInternal(notification.ProductId, cancellationToken);

    public async Task Handle(ProductTagsReplacedDomainEvent notification, CancellationToken cancellationToken) =>
         await HandleInternal(notification.ProductId, cancellationToken);

    private async Task HandleInternal(Guid id, CancellationToken cancellationToken) =>
      await cache.RemoveByTagAsync(CacheTags.ProductById(id), token: cancellationToken);
}
