using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;

public sealed class UpdateVendorSnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient,
    ICatalogMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateVendorSnapshotCommand>
{
    public async Task Handle(UpdateVendorSnapshotCommand request, CancellationToken cancellationToken)
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
            metrics.RecordSnapshotOutdated(nameof(UpdateVendorSnapshotCommand));
            return;
        }

        var isActiveChanged = snapshot.IsActive != request.IsActive;

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        snapshot.IsActive = request.IsActive;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);

        if (isActiveChanged)
        {
            backgroundJobClient.Enqueue<ISuspendVendorProductsJob>(
                job => job.Execute(request.VendorId, request.IsActive, CancellationToken.None));
        }

        await transaction.CommitAsync(cancellationToken);
    }
}