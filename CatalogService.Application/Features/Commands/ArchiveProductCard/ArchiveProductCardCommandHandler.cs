using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ArchiveProductCard;

public sealed class ArchiveProductCardCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<ArchiveProductCardCommand, IResult>
{
    public async Task<IResult> Handle(ArchiveProductCardCommand request, CancellationToken cancellationToken)
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

        var ctx = new ProductCardArchivedContext(vendorSnapshot.IsActive);

        var result = productCard.Archive(ctx, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
