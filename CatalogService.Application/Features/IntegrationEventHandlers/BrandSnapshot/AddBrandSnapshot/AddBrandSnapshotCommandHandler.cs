using CatalogService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.AddBrandSnapshot;

public sealed class AddBrandSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddBrandSnapshotCommand>
{
    public async Task Handle(AddBrandSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.BrandSnapshots
            .AnyAsync(x => x.BrandId == request.BrandId, cancellationToken);
        if (exists)
            return;

        context.BrandSnapshots.Add(
            new Domain.Snapshots.BrandSnapshot
            {
                BrandId = request.BrandId,
                IsAssignable = request.IsAssignable,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
