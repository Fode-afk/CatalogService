using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;

public sealed class UpdateProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductVariantSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantSnapshots
         .FirstOrDefaultAsync(v => v.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return Fail(ProductVariantSnapshotErrors.NotFound());

        if (request.Version <= snapshot.Version)
            return Ok();

        if (snapshot.HasMainImage == request.HasMainImage)
        {
            snapshot.Version = request.Version;
            snapshot.UpdatedAt = timeProvider.GetUtcNow();

            await context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        snapshot.HasMainImage = request.HasMainImage;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
