using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.CreateProductCard;

public sealed class CreateProductCardCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<CreateProductCardCommand, IResult>
{
    public async Task<IResult> Handle(CreateProductCardCommand request, CancellationToken cancellationToken)
    {
        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

        var categorySnapshot = await context.CategorySnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.CategoryId == request.CategoryId, cancellationToken);
        if (categorySnapshot is null)
            return Fail(CategorySnapshotErrors.NotFound());

        var buildResult = ProductCardCreationDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return buildResult;

        var data = buildResult.Value;

        var ctx = new ProductCardCreationContext(
            vendorSnapshot.IsActive,
            categorySnapshot.IsActive,
            data.Attributes,
            data.Tags);

        var result = ProductCard.Create(
            ctx,
            data,
            request.CategoryId,
            request.VendorId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        context.ProductCards.Add(result.Value);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}