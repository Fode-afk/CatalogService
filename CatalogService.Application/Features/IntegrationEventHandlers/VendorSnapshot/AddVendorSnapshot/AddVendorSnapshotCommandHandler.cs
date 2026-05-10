using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;

public sealed class AddVendorSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddVendorSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.VendorSnapshots
            .AnyAsync(x => x.VendorId == request.VendorId, cancellationToken);
        if (exists)
            return Ok();

        context.VendorSnapshots.Add(
            new Domain.Snapshots.VendorSnapshot
            {        
                VendorId = request.VendorId,
                IsActive = request.IsActive,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}