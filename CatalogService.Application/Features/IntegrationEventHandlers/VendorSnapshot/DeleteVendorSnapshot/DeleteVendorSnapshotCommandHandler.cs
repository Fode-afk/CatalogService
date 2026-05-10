using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;

public sealed class DeleteVendorSnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobs) : IRequestHandler<DeleteVendorSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(DeleteVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.VendorSnapshots
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (snapshot is null)
            return Ok();

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.VendorSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        backgroundJobs.Enqueue<IDeleteVendorProductsJob>(
            job => job.Execute(request.VendorId, CancellationToken.None));

        await transaction.CommitAsync(cancellationToken);

        return Ok();
    }
}
