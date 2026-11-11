using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Microsoft.EntityFrameworkCore;
namespace CatalogService.Infrastructure.Jobs;

internal sealed class RestrictVendorProductsMarketsJob(
    IAppDbContext context,
    TimeProvider timeProvider) : IRestrictVendorProductsMarketsJob
{
    public async Task Execute(Guid vendorId, CancellationToken cancellationToken = default)
    {
        var snapshot = await context.VendorSnapshots.AsNoTracking()
            .FirstAsync(v => v.VendorId == vendorId, cancellationToken);
        var allowed = snapshot.Markets.ToHashSet();

        var lastId = Guid.Empty;
        while (true)
        {
            var products = await context.Products
                .Where(p => p.VendorId == vendorId && p.Id > lastId)
                .OrderBy(p => p.Id)
                .Take(100)
                .ToListAsync(cancellationToken);

            if (products.Count == 0) 
                break;

            foreach (var product in products)
                product.RestrictMarkets(allowed, timeProvider.GetUtcNow());

            await context.SaveChangesAsync(cancellationToken);
            context.ClearChangeTracker();
            lastId = products[^1].Id;
        }
    }
}
