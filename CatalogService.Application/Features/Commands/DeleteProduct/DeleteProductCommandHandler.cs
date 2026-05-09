using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<DeleteProductCommand, IResult>
{
    public async Task<IResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
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

        var ctx = new ProductDeleteContext(
            vendorSnapshot.IsActive,
            product.CanBeModified);

        var result = product.Delete(ctx, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}