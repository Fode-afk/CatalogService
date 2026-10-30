using CatalogService.Application.Dtos;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UnpublishProduct;

public sealed class UnpublishProductCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UnpublishProductCommand, IResult<ProductWriteAck>>
{
    public async Task<IResult<ProductWriteAck>> Handle(UnpublishProductCommand request, CancellationToken cancellationToken)
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

        var ctx = new ProductUnpublishContext(
            vendorSnapshot.IsActive,
            product.CanEditOperationalData);

        var result = product.Unpublish(ctx, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return Fail<ProductWriteAck>(result.Error);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new ProductWriteAck(product.Id, product.Version));
    }
}
