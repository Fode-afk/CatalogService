using CatalogService.Application.Dtos;
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
    TimeProvider timeProvider) : IRequestHandler<UpdateProductInfoCommand, IResult<ProductWriteAck>>
{
    public async Task<IResult<ProductWriteAck>> Handle(UpdateProductInfoCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(c => c.Id == request.ProductId, cancellationToken);
        if (product == null)
            return Fail<ProductWriteAck>(ProductErrors.NotFound());

        var ownershipCtx = new ProductVendorOwnershipContext(
           request.VendorId,
           product.VendorId);

        var ownershipResult = ProductBelongsToVendorSpec.Instance.IsSatisfiedBy(ownershipCtx);
        if (ownershipResult.IsFailure)
            return Fail<ProductWriteAck>(ownershipResult.Error);

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot == null)
            return Fail<ProductWriteAck>(VendorSnapshotErrors.NotFound());

        var categorySnapshot = await context.CategorySnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.CategoryId == request.CategoryId, cancellationToken);
        if (categorySnapshot is null)
            return Fail<ProductWriteAck>(CategorySnapshotErrors.NotFound());

        var brandSnapshot = await context.BrandSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);
        if (brandSnapshot is null)
            return Fail<ProductWriteAck>(BrandSnapshotErrors.NotFound());

        var buildResult = ProductUpdateInfoDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return Fail<ProductWriteAck>(buildResult.Error);

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
            return Fail<ProductWriteAck>(result.Error);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Fail<ProductWriteAck>(SlugErrors.NotUnique());
        }

        return Ok(new ProductWriteAck(product.Id, product.Version));
    }
}