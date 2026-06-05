using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using migApp.Shared.Enums.Products;

namespace CatalogService.Infrastructure.Observability;

public sealed class ActiveProductsMetricCollector(
    IServiceScopeFactory scopeFactory,
    ICatalogMetrics metrics,
    ILogger<ActiveProductsMetricCollector> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider
                    .GetRequiredService<IAppDbContext>();

                var count = await context.Products
                    .CountAsync(p => p.ProductStatus == ProductStatus.Published, cancellationToken);

                metrics.SetActiveProductsCount(count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to collect active products count");
            }
        }
    }
}