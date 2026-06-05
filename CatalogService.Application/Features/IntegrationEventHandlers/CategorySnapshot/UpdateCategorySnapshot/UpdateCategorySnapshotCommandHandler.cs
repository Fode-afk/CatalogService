using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.UpdateCategorySnapshot;

public sealed class UpdateCategorySnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient,
    ICatalogMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateCategorySnapshotCommand>
{
    public async Task Handle(UpdateCategorySnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.CategorySnapshots
           .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);

        if (snapshot == null)
        {
            metrics.RecordSnapshotNotFound("Category");
            throw new SnapshotNotFoundException("Category", request.CategoryId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateCategorySnapshotCommand));
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
            backgroundJobClient.Enqueue<ISuspendCategoryProductsJob>(
                job => job.Execute(request.CategoryId, request.IsActive, CancellationToken.None));
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
