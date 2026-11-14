using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Infrastructure.Jobs;

internal sealed class CleanupSoftDeletedProductsJob(
    IAppDbContext context,
    ILogger<CleanupSoftDeletedProductsJob> logger,
    TimeProvider timeProvider,
    SoftDeletedProductsCleanupOptions options) : ICleanupSoftDeletedProductsJob
{
    [DisableConcurrentExecution(timeoutInSeconds: 60 * 30)]
    [AutomaticRetry(Attempts = 0)]
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var cutoff = timeProvider.GetUtcNow() - options.RetentionPeriod;
        long totalDeleted = 0;

        long deletedInBatch;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ids = await context.Products
                .IgnoreQueryFilters()
                .Where(p => p.DeletedAt != null && p.DeletedAt <= cutoff)
                .OrderBy(p => p.Id)
                .Select(p => p.Id)
                .Take(options.BatchSize)
                .ToListAsync(cancellationToken);

            if (ids.Count == 0)
                break;

            deletedInBatch = await context.Products
                .IgnoreQueryFilters()
                .Where(p => ids.Contains(p.Id))
                .ExecuteDeleteAsync(cancellationToken);

            totalDeleted += deletedInBatch;

            if (deletedInBatch > 0)
                await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        }
        while (deletedInBatch == options.BatchSize);

        if (totalDeleted > 0)
            logger.LogInformation(
                "Cleanup soft-deleted products: removed {Count} records older than {Cutoff:u}",
                totalDeleted, cutoff);
    }
}