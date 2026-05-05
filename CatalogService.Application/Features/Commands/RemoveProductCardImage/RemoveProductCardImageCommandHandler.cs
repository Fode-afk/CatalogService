using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.RemoveProductCardImage;

public sealed class RemoveProductCardImageCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<RemoveProductCardImageCommand, IResult>
{
    public async Task<IResult> Handle(RemoveProductCardImageCommand request, CancellationToken cancellationToken)
    {
        var productCard = await context.ProductCards
           .FirstOrDefaultAsync(p => p.Id == request.ProductCardId, cancellationToken);
        if (productCard == null)
            return Fail(ProductCardErrors.NotFound());

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot == null)
            return Fail(VendorSnapshotErrors.NotFound());

        var imageUrlResult = ImageUrl.Create(request.Url);
        if (imageUrlResult.IsFailure)
            return imageUrlResult;

        var ctx = new ProductCardRemoveImageContext(
            vendorSnapshot.IsActive,
            productCard.ProductCardStatus);

        var result = productCard.RemoveProductCardImage(
            ctx,
            imageUrlResult.Value, 
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
