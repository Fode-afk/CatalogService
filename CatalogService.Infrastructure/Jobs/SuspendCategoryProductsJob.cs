using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Models;
using migApp.Shared.Enums.Products;
using Microsoft.EntityFrameworkCore;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using Microsoft.Extensions.Logging;

namespace CatalogService.Infrastructure.Jobs;

internal sealed class SuspendCategoryProductsJob(
    IAppDbContext context, 
    ICatalogMetrics metrics,
    ILogger<SuspendCategoryProductsJob> logger,
    TimeProvider timeProvider) : ISuspendCategoryProductsJob
{
    public async Task Execute(Guid categoryId, bool isActive, CancellationToken cancellationToken = default)
    {
        var suspensionReason = new ProductSuspensionReason(SuspensionReason.CategoryDeactivated);

        const int batchSize = 100;
        var offset = 0;

        while (true)
        {
            var products = await context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Id)
                .Skip(offset)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (products.Count == 0)
                break;

            foreach (var product in products)
            {
                if (!isActive)
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
                else
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
            }

            await context.SaveChangesAsync(cancellationToken);

            if (products.Count < batchSize)
                break;

            offset += batchSize;
            context.ClearChangeTracker();
        }
    }
}
