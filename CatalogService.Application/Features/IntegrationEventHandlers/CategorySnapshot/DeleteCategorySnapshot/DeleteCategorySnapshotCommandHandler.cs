using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.DeleteCategorySnapshot;

public sealed class DeleteCategorySnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient) : IRequestHandler<DeleteCategorySnapshotCommand>
{
    public async Task Handle(DeleteCategorySnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.CategorySnapshots
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (snapshot is null)
            return;

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.CategorySnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<ISuspendCategoryProductsJob>(
            job => job.Execute(request.CategoryId, isActive: false, CancellationToken.None));

        await transaction.CommitAsync(cancellationToken);
    }
}
