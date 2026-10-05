using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductInfo;

public sealed class UpdateProductInfoCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductInfoCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductInfoCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(c => c.Id == request.ProductId, cancellationToken);
        if (product == null)
            return Fail(ProductErrors.NotFound());

        var ownershipCtx = new ProductVendorOwnershipContext(
           request.VendorId,
           product.VendorId);

        var ownershipResult = ProductBelongsToVendorSpec.Instance.IsSatisfiedBy(ownershipCtx);
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

        var brandSnapshot = await context.BrandSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);
        if (brandSnapshot is null)
            return Fail(BrandSnapshotErrors.NotFound());

        var buildResult = ProductUpdateInfoDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return buildResult;

        var ctx = new ProductUpdateInfoContext(
            vendorSnapshot.IsActive,
            product.CanEditContent,
            categorySnapshot.IsActive,
            brandSnapshot.IsAssignable);

        var result = product.UpdateInfo(
            ctx,
            buildResult.Value,
            request.CategoryId,
            request.BrandId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Fail(SlugErrors.NotUnique());
        }

        return Ok();
    }
}