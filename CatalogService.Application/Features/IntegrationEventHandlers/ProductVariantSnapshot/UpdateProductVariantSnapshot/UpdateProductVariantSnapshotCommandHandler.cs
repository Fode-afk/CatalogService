using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;

public sealed class UpdateProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    ICatalogMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductVariantSnapshotCommand>
{
    public async Task Handle(UpdateProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantSnapshots
            .FirstOrDefaultAsync(v => v.ProductVariantId == request.ProductVariantId, cancellationToken);

        if (snapshot == null)
        {
            metrics.RecordSnapshotNotFound("ProductVariant");
            throw new SnapshotNotFoundException("ProductVariant", request.ProductVariantId);
        }

        if (request.Version <= snapshot.Version)
        {
            metrics.RecordSnapshotOutdated(nameof(UpdateProductVariantSnapshotCommand));
            return;
        }

        snapshot.HasMainImage = request.HasMainImage;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();
        snapshot.Version = request.Version;

        await context.SaveChangesAsync(cancellationToken);
    }
}