using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Domain.ValueObjects;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;

public sealed class AddVendorSnapshotCommandHandler(
    IAppDbContext context,
    ILogger<AddVendorSnapshotCommandHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<AddVendorSnapshotCommand>
{
    public async Task Handle(AddVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.VendorSnapshots
            .AnyAsync(x => x.VendorId == request.VendorId, cancellationToken);
        if (exists)
            return;

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

        context.VendorSnapshots.Add(
            new Domain.Snapshots.VendorSnapshot
            {        
                VendorId = request.VendorId,
                IsActive = request.IsActive,
                Markets = markets,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}