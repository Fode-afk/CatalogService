using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ReplaceProductAttributes;

public sealed class ReplaceProductAttributesCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<ReplaceProductAttributesCommand, IResult>
{
    public async Task<IResult> Handle(ReplaceProductAttributesCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
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

        var characteristicIds = request.Attributes.Keys.ToList();
        var characteristicSnapshots = await context.CharacteristicSnapshots
            .AsNoTracking()
            .Where(c =>
                c.CategoryId == product.CategoryId &&
                characteristicIds.Contains(c.CharacteristicId))
            .ToListAsync(cancellationToken);
        if (characteristicSnapshots.Count == 0)
            return Fail(CharacteristicSnapshotErrors.NotFound());

        var buildResult = ProductReplaceAttributesDataBuilder.Build(request, characteristicSnapshots);
        if (buildResult.IsFailure)
            return buildResult;

        var ctx = new ProductAttributesReplaceContext(
            vendorSnapshot.IsActive,
            product.CanEditContent,
            buildResult.Value);

        var result = product.ReplaceAttributes(
            ctx,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}