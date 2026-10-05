using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;

public sealed class DeleteProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider,
    ICatalogMetrics metrics,
    ILogger<DeleteProductVariantSnapshotCommandHandler> logger) : IRequestHandler<DeleteProductVariantSnapshotCommand>
{
    public async Task Handle(DeleteProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantSnapshots
            .FirstOrDefaultAsync(v => v.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return;

        var product = await context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == snapshot.ProductId, cancellationToken);

        if (product is null || product.IsDeleted)
        {
            context.ProductVariantSnapshots.Remove(snapshot);
            await context.SaveChangesAsync(cancellationToken);
            return;
        }

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == product.VendorId, cancellationToken);

        if (vendorSnapshot == null)
        {
            metrics.RecordSnapshotNotFound("Vendor");
            throw new SnapshotNotFoundException("Vendor", product.VendorId);
        }

        var ctx = new ProductRemoveVariantFromAttributesContext(
            vendorSnapshot.IsActive,
            product.CanEditContent);

        var result = product.RemoveVariantFromAttributes(
            ctx,
            request.ProductVariantId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            logger.RemoveVariantFromAttributesFailed(
                request.ProductVariantId, snapshot.ProductId, result.Error.Message);

        context.ProductVariantSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}