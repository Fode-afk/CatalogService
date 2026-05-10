using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;

public sealed class DeleteProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<DeleteProductVariantSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(DeleteProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantSnapshots
            .FirstOrDefaultAsync(v => v.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return Ok();

        var product = await context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == snapshot.ProductId, cancellationToken);

        if (product is null || product.IsDeleted)
        {
            context.ProductVariantSnapshots.Remove(snapshot);
            await context.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == product.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

        var ctx = new ProductRemoveVariantFromAttributesContext(
            vendorSnapshot.IsActive,
            product.CanBeModified);

        var result = product.RemoveVariantFromAttributes(
            ctx,
            request.ProductVariantId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        context.ProductVariantSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}