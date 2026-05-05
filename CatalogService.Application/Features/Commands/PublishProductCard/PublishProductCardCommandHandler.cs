using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.PublishProductCard;

public sealed class PublishProductCardCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<PublishProductCardCommand, IResult>
{
    public async Task<IResult> Handle(PublishProductCardCommand request, CancellationToken cancellationToken)
    {
        var productCard = await context.ProductCards
            .FirstOrDefaultAsync(c => c.Id == request.ProductCardId, cancellationToken);
        if (productCard == null)
            return Fail(ProductCardErrors.NotFound());

        var ownershipCtx = new ProductCardVendorOwnershipContext(
            request.VendorId,
            productCard.VendorId);

        var ownershipResult = ProductCardBelongsToVendorSpec.Instance.IsSatisfiedBy(ownershipCtx);
        if (ownershipResult.IsFailure)
            return ownershipResult;

        var vendorSnapshot = await context.VendorSnapshots
           .AsNoTracking()
           .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot == null)
            return Fail(VendorSnapshotErrors.NotFound());

        var productSnapshot = await context.ProductSnapshots
           .AsNoTracking()
           .FirstOrDefaultAsync(v => v.ProductId == productCard.DefaultProductId, cancellationToken);
        if (productSnapshot == null)
            return Fail(ProductSnapshotErrors.NotFound());

        var priceSnapshot = await context.ProductPriceSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.ProductId == productSnapshot.ProductId, cancellationToken);
        if (priceSnapshot == null)
            return Fail(ProductPriceSnapshotErrors.NotFound());

        var inventorySnapshot = await context.ProductInventorySnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.ProductId == productSnapshot.ProductId, cancellationToken);
        if (inventorySnapshot == null)
            return Fail(ProductInventorySnapshotErrors.NotFound());

        var ctx = new ProductCardPublishContext(
            vendorSnapshot.IsActive,
            productSnapshot is not null,
            priceSnapshot.HasPrice,
            inventorySnapshot.InStock,
            productCard.ProductCardStatus,
            productCard.ProductCount,
            productCard.Images);

        var result = productCard.Publish(
            ctx,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
