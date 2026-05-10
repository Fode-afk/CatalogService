using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Enums.Products;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;

public sealed class DeleteProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<DeleteProductVariantPriceSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(DeleteProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantPriceSnapshots
           .FirstOrDefaultAsync(p => p.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return Ok();

        var product = await context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == snapshot.ProductId, cancellationToken);

        if (product is null || product.IsDeleted)
        {
            context.ProductVariantPriceSnapshots.Remove(snapshot);
            await context.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        var hasAnyPrice = await context.ProductVariantPriceSnapshots
            .AnyAsync(p =>
                p.ProductId == snapshot.ProductId &&
                p.ProductVariantId != request.ProductVariantId &&
                p.HasPrice,
                cancellationToken);

        var suspensionReason = new ProductSuspensionReason(SuspensionReason.NoPriceAvailable);

        if (!hasAnyPrice)
        {
            product.Suspend(
                new ProductSuspendContext(product.CanBeModified),
                suspensionReason,
                timeProvider.GetUtcNow());
        }

        context.ProductVariantPriceSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
