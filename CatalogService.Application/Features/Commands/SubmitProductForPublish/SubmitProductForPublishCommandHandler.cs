using CatalogService.Application.Dtos;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.SubmitProductForPublish;

public sealed class SubmitProductForPublishCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<SubmitProductForPublishCommand, IResult<ProductWriteAck>>
{
    public async Task<IResult<ProductWriteAck>> Handle(SubmitProductForPublishCommand request, CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId, cancellationToken);
        if (categorySnapshot == null)
            return Fail<ProductWriteAck>(CategorySnapshotErrors.NotFound());

        var brandSnapshot = await context.BrandSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BrandId == product.BrandId, cancellationToken);
        if (brandSnapshot == null)
            return Fail<ProductWriteAck>(BrandSnapshotErrors.NotFound());

        var variationSnapshots = await context.ProductVariantSnapshots
            .AsNoTracking()
            .Where(v => v.ProductId == product.Id)
            .ToListAsync(cancellationToken);
        if (variationSnapshots.Count == 0)
            return Fail<ProductWriteAck>(ProductErrors.NoVariations());

        var priceSnapshots = await context.ProductVariantPriceSnapshots
            .AsNoTracking()
            .Where(p => variationSnapshots.Select(v => v.ProductVariantId).Contains(p.ProductVariantId))
            .ToListAsync(cancellationToken);

        var ctx = new ProductSubmitForPublishContext(
            vendorSnapshot.IsActive,
            categorySnapshot.IsActive,
            brandSnapshot.IsAssignable,
            product.CanEditContent,
            product.Attributes,
            product.Tags,
            variationSnapshots,
            priceSnapshots);

        var result = product.SubmitForPublish(
            ctx,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return Fail<ProductWriteAck>(result.Error);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new ProductWriteAck(product.Id, product.Version));
    }
}
