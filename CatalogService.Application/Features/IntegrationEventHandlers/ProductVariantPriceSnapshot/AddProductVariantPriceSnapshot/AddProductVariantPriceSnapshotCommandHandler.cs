using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;

public sealed class AddProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductVariantPriceSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductVariantPriceSnapshots
            .AnyAsync(x => x.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (exists)
            return Ok();

        context.ProductVariantPriceSnapshots.Add(
            new Domain.Snapshots.ProductVariantPriceSnapshot
            {
                ProductVariantId = request.ProductVariantId,
                HasPrice = request.HasPrice,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
