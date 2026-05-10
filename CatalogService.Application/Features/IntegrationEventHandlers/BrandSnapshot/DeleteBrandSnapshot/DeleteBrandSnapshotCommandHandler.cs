using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;

public sealed class DeleteBrandSnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient) : IRequestHandler<DeleteBrandSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(DeleteBrandSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.BrandSnapshots
            .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);
        if (snapshot is null)
            return Ok();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.BrandSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<ISuspendBrandProductsJob>(
            job => job.Execute(request.BrandId, isActive: false, CancellationToken.None));

        await transaction.CommitAsync(cancellationToken);

        return Ok();
    }
}
