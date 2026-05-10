using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Domain.Errors;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.UpdateCategorySnapshot;

public sealed class UpdateCategorySnapshotCommandHandler(
    IAppDbContext context,
    IBackgroundJobClient backgroundJobClient,
    TimeProvider timeProvider) : IRequestHandler<UpdateCategorySnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateCategorySnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.CategorySnapshots
           .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (snapshot is null)
            return Fail(BrandSnapshotErrors.NotFound());

        if (request.Version <= snapshot.Version)
            return Ok();

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

        return Ok();
    }
}
