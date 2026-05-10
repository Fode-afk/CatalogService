using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.AddBrandSnapshot;

public sealed class AddBrandSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddBrandSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddBrandSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.BrandSnapshots
            .AnyAsync(x => x.BrandId == request.BrandId, cancellationToken);
        if (exists)
            return Ok();

        context.BrandSnapshots.Add(
            new Domain.Snapshots.BrandSnapshot
            {
                BrandId = request.BrandId,
                IsActive = request.IsActive,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
