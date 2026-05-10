using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Jobs;

internal sealed class DeleteVendorProductsJob(
    IAppDbContext context,
    TimeProvider timeProvider) : IDeleteVendorProductsJob
{
    public async Task Execute(Guid vendorId, CancellationToken cancellationToken)
    {
        const int batchSize = 100;
        var offset = 0;

        while (true)
        {
            var products = await context.Products
                .Where(p => p.VendorId == vendorId && !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Skip(offset)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (products.Count == 0)
                break;

            foreach (var product in products)
                product.ForceDelete(timeProvider.GetUtcNow());

            await context.SaveChangesAsync(cancellationToken);

            if (products.Count < batchSize)
                break;

            offset += batchSize;
            context.ChangeTracker.Clear();
        }
    }
}
