using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;

public sealed class AddProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    ILogger<AddProductVariantPriceSnapshotCommandHandler> logger) : IRequestHandler<AddProductVariantPriceSnapshotCommand>
{
    public async Task Handle(AddProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductVariantPriceSnapshots
            .AnyAsync(x => x.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (exists)
            return;

        var marketResult = Market.Create(request.Market);
        if (marketResult.IsFailure)
        {
            logger.LogWarning(
                  "Failed to create Market from code {MarketCode}. Error: {Error}",
                  request.Market,
                  marketResult.Error);
            return;
        }

        context.ProductVariantPriceSnapshots.Add(
            new Domain.Snapshots.ProductVariantPriceSnapshot
            {
                PriceId = request.PriceId,
                ProductVariantId = request.ProductVariantId,
                Market = marketResult.Value
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}