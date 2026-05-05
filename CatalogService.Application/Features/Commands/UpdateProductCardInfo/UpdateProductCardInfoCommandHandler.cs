using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductCardInfo;

public sealed class UpdateProductCardInfoCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductCardInfoCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductCardInfoCommand request, CancellationToken cancellationToken)
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

        var categorySnapshot = await context.CategorySnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.CategoryId == request.CategoryId, cancellationToken);
        if (categorySnapshot is null)
            return Fail(CategorySnapshotErrors.NotFound());

        var buildResult = ProductCardUpdateInfoDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return buildResult;

        var ctx = new ProductCardUpdateInfoContext(
            vendorSnapshot.IsActive,
            productCard.ProductCardStatus);

        var result = productCard.UpdateInfo(
            ctx,
            buildResult.Value,
            request.CategoryId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
