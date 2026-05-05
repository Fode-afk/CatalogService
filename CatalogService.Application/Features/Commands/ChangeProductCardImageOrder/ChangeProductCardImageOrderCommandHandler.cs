using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using CatalogService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ChangeProductCardImageOrder;

public sealed class ChangeProductCardImageOrderCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<ChangeProductCardImageOrderCommand, IResult>
{
    public async Task<IResult> Handle(ChangeProductCardImageOrderCommand request, CancellationToken cancellationToken)
    {
        var productCard = await context.ProductCards
            .FirstOrDefaultAsync(p => p.Id == request.ProductCardId, cancellationToken);
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

        var imageUrlResult = ImageUrl.Create(request.Url);
        if (imageUrlResult.IsFailure)
            return imageUrlResult;

        var ctx = new ProductCardChangeImageOrderContext(
            vendorSnapshot.IsActive,
            productCard.ProductCardStatus);

        var result = productCard.ChangeImageOrder(
            ctx,
            imageUrlResult.Value,
            request.NewOrder,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
