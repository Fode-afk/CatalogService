using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.AddVendorSnapshot;

public sealed class AddVendorSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddVendorSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddVendorSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.VendorSnapshots
            .AnyAsync(x => x.VendorId == request.VendorId, cancellationToken);

        if (exists)
            return Fail(VendorSnapshotErrors.AlreadyExists());

        context.VendorSnapshots.Add(
            new VendorSnapshot
            {        
                VendorId = request.VendorId,
                UpdatedAt = timeProvider.GetUtcNow()
            });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
