using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;

public sealed class DeleteBrandSnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient) : IRequestHandler<DeleteBrandSnapshotCommand>
{
    public async Task Handle(DeleteBrandSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.BrandSnapshots
            .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);
        if (snapshot is null)
            return;

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.BrandSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<ISuspendBrandProductsJob>(
            job => job.Execute(request.BrandId, isActive: false, CancellationToken.None));

        await transaction.CommitAsync(cancellationToken);
    }
}
