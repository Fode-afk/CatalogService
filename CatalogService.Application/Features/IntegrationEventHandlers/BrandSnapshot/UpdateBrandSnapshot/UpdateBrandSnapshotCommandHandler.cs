using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.UpdateBrandSnapshot;

public sealed class UpdateBrandSnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient,
    ICatalogMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateBrandSnapshotCommand>
{
    public async Task Handle(UpdateBrandSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.BrandSnapshots
           .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);

        if (snapshot == null)
        {
            metrics.RecordSnapshotNotFound("Brand");
            throw new SnapshotNotFoundException("Brand", request.BrandId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateBrandSnapshotCommand));
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
            backgroundJobClient.Enqueue<ISuspendBrandProductsJob>(
                job => job.Execute(request.BrandId, request.IsActive, CancellationToken.None));
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
