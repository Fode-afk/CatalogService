using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.AddCategorySnapshot;

public sealed class AddCategorySnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddCategorySnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddCategorySnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.CategorySnapshots
            .AnyAsync(x => x.CategoryId == request.CategoryId, cancellationToken);
        if (exists)
            return Ok();

        context.CategorySnapshots.Add(
            new Domain.Snapshots.CategorySnapshot
            {
                CategoryId = request.CategoryId,
                IsActive = request.IsActive,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
