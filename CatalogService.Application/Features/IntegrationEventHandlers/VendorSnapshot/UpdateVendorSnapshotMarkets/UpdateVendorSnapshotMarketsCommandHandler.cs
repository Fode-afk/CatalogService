using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorMarkets;

public sealed class UpdateVendorSnapshotMarketsCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient,
    ICatalogMetrics metrics,
    ILogger<UpdateVendorSnapshotMarketsCommandHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<UpdateVendorSnapshotMarketsCommand>
{
    public async Task Handle(UpdateVendorSnapshotMarketsCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);

        if (snapshot == null)
        {
            metrics.RecordSnapshotNotFound("Vendor");
            throw new SnapshotNotFoundException("Vendor", request.VendorId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateVendorSnapshotMarketsCommand));
            return;
        }

        List<Market> markets = [];

        foreach (var marketCode in request.Markets)
        {
            var marketResult = Market.Create(marketCode);

            if (marketResult.IsFailure)
            {
                logger.LogWarning(
                   "Failed to create Market from code {MarketCode}. Error: {Error}",
                   marketCode,
                   marketResult.Error);
                return;
            }

            markets.Add(marketResult.Value);
        }

        var previous = snapshot.Markets.ToHashSet();
        var current = markets.ToHashSet();
        var removed = previous.Except(current).ToList();

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        snapshot.Markets = markets;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);

        if (removed.Count > 0)
            backgroundJobClient.Enqueue<IRestrictVendorProductsMarketsJob>(
                job => job.Execute(request.VendorId, CancellationToken.None));

        await transaction.CommitAsync(cancellationToken);
    }
}
