using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Enums.Products;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.UpdateProductVariantPriceSnapshot;

public sealed class UpdateProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider,
    ICatalogMetrics metrics,
    ILogger<UpdateProductVariantPriceSnapshotCommandHandler> logger) : IRequestHandler<UpdateProductVariantPriceSnapshotCommand>
{
    public async Task Handle(UpdateProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantPriceSnapshots
            .FirstOrDefaultAsync(p => p.ProductVariantId == request.ProductVariantId, cancellationToken);

        if (snapshot == null)
        {
            metrics.RecordSnapshotNotFound("ProductVariant");
            throw new SnapshotNotFoundException("ProductVariant", request.ProductVariantId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateProductVariantPriceSnapshotCommand));
            return;
        }

        snapshot.HasPrice = request.HasPrice;
        snapshot.Version = request.Version;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();

        var productId = snapshot.ProductId;

        var hasAnyPrice = await context.ProductVariantPriceSnapshots
            .AnyAsync(p =>
                p.ProductId == productId &&
                p.ProductVariantId != request.ProductVariantId &&
                p.HasPrice,
                cancellationToken);

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product == null)
        {
            metrics.RecordProductNotFound();
            throw new ProductNotFoundException(productId);
        }

        var suspensionReason = new ProductSuspensionReason(SuspensionReason.NoPriceAvailable);

        if (!request.HasPrice && !hasAnyPrice)
        {
            var result = product.Suspend(
                new ProductSuspendContext(product.CanBeModified),
                suspensionReason,
                timeProvider.GetUtcNow());

            if (result.IsSuccess)
                metrics.RecordProductSuspended(suspensionReason.Reason.ToString());
            else
                logger.ProductSuspendFailed(product.Id, result.Error.Message);
        }
        else if (request.HasPrice)
        {
            var result = product.TryRestore(
                new ProductTryRestoreContext(product.ProductStatus),
                suspensionReason,
                timeProvider.GetUtcNow());

            if (result.IsSuccess)
                metrics.RecordProductRestored();
            else
                logger.ProductRestoreFailed(product.Id, result.Error.Message);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
