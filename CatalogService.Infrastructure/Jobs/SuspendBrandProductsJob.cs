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

internal sealed class SuspendBrandProductsJob(
    IAppDbContext context,
    ICatalogMetrics metrics,
    ILogger<SuspendBrandProductsJob> logger,
    TimeProvider timeProvider) : ISuspendBrandProductsJob
{
    public async Task Execute(Guid brandId, bool isActive, CancellationToken cancellationToken = default)
    {
        var suspensionReason = new ProductSuspensionReason(SuspensionReason.BrandDeactivated);
        const int batchSize = 100;
        var processedIds = new HashSet<Guid>();

        while (true)
        {
            var products = await context.Products
                .Where(p => p.BrandId == brandId && !processedIds.Contains(p.Id))
                .OrderBy(p => p.Id)
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

                processedIds.Add(product.Id);
            }

            await context.SaveChangesAsync(cancellationToken);
            context.ClearChangeTracker();

            if (products.Count < batchSize)
                break;
        }
    }
}