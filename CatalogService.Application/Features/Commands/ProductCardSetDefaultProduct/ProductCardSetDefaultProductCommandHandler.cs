using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ProductCardSetDefaultProduct;

public sealed class ProductCardSetDefaultProductCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<ProductCardSetDefaultProductCommand, IResult>
{
    public async Task<IResult> Handle(ProductCardSetDefaultProductCommand request, CancellationToken cancellationToken)
    {
        var productCard = await context.ProductCards.FirstOrDefaultAsync(p =>
            p.Id == request.ProductCardId,
            cancellationToken);
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

        var defaultProduct = await context.ProductSnapshots.FirstOrDefaultAsync(p =>
            p.ProductId == request.DefaultProductId,
            cancellationToken);
        if (defaultProduct == null)
            return Fail(ProductSnapshotErrors.NotFound());

        var ctx = new ProductCardSetDefaultProductContext(
            vendorSnapshot.IsActive,
            productCard.ProductCardStatus,
            defaultProduct.ProductCardId == productCard.Id);

        var result = productCard.SetDefaultProduct(
            ctx,
            request.DefaultProductId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
