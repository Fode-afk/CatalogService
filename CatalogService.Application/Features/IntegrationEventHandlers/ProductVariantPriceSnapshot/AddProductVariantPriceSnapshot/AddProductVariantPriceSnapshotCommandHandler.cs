using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;

public sealed class AddProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductVariantPriceSnapshotCommand>
{
    public async Task Handle(AddProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductVariantPriceSnapshots
            .AnyAsync(x => x.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (exists)
            return;

        context.ProductVariantPriceSnapshots.Add(
            new Domain.Snapshots.ProductVariantPriceSnapshot
            {
                ProductVariantId = request.ProductVariantId,
                HasPrice = request.HasPrice,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
