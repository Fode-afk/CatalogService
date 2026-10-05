using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Enums.Products;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;

public sealed class DeleteProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider,
    ICatalogMetrics metrics,
    ILogger<DeleteProductVariantPriceSnapshotCommandHandler> logger) : IRequestHandler<DeleteProductVariantPriceSnapshotCommand>
{
    public async Task Handle(DeleteProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantPriceSnapshots
           .FirstOrDefaultAsync(p => p.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return;

        var product = await context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == snapshot.ProductId, cancellationToken);

        if (product is null || product.IsDeleted)
        {
            context.ProductVariantPriceSnapshots.Remove(snapshot);
            await context.SaveChangesAsync(cancellationToken);
            return;
        }

        var hasAnyPrice = await context.ProductVariantPriceSnapshots
            .AnyAsync(p =>
                p.ProductId == snapshot.ProductId &&
                p.ProductVariantId != request.ProductVariantId &&
                p.HasPrice,
                cancellationToken);

        var suspensionReason = new ProductSuspensionReason(SuspensionReason.NoPriceAvailable);

        if (!hasAnyPrice)
        {
            var result = product.Suspend(
                new ProductSuspendContext(product.CanEditContent),
                suspensionReason,
                timeProvider.GetUtcNow());

            if (result.IsSuccess)
                metrics.RecordProductSuspended(suspensionReason.Reason.ToString());
            else
                logger.ProductSuspendFailed(product.Id, result.Error.Message);
        }

        context.ProductVariantPriceSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}
