using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Enums.Products;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.UpdateProductVariantPriceSnapshot;

public sealed class UpdateProductVariantPriceSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductVariantPriceSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantPriceSnapshots
            .FirstOrDefaultAsync(p => p.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (snapshot is null)
            return Fail(ProductVariantPriceSnapshotErrors.NotFound());

        if (request.Version <= snapshot.Version)
            return Ok();

        if (snapshot.HasPrice == request.HasPrice)
        {
            snapshot.Version = request.Version;
            snapshot.UpdatedAt = timeProvider.GetUtcNow();

            await context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        snapshot.HasPrice = request.HasPrice;
        snapshot.Version = request.Version;
        snapshot.UpdatedAt = timeProvider.GetUtcNow();

        var productId = snapshot.ProductId;

        var hasAnyPrice = await context.ProductVariantPriceSnapshots
            .AnyAsync(p =>
                p.ProductId == productId &&
                p.ProductVariantId != request.ProductVariantId &&
                p.HasPrice,
                cancellationToken);

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product is null)
            return Fail(ProductErrors.NotFound());

        var suspensionReason = new ProductSuspensionReason(SuspensionReason.NoPriceAvailable);

        if (!request.HasPrice && !hasAnyPrice)
        {
            product.Suspend(
                new ProductSuspendContext(product.CanBeModified),
                suspensionReason,
                timeProvider.GetUtcNow());
        }
        else if (request.HasPrice)
        {
            product.TryRestore(
                new ProductTryRestoreContext(product.ProductStatus),
                suspensionReason,
                timeProvider.GetUtcNow());
        }

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
