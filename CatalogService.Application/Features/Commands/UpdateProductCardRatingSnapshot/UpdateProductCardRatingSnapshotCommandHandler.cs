using CatalogService.Application.Caching;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Services;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductCardRatingSnapshot;

public sealed class UpdateProductCardRatingSnapshotCommandHandler(
    IAppDbContext context,
    IFusionCache cache,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductCardRatingSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductCardRatingSnapshotCommand request, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductReadModels
            .FirstOrDefaultAsync(c => c.Id == request.ProductCardId, cancellationToken);

        if (productCardReadModel == null)
            return Fail(ProductErrors.NotFound());

        if (request.RatingCount > 0 && (request.RatingAvg < 1 || request.RatingAvg > 5))
            return Fail(RatingSnapshotErrors.OutOfRange());

        if (request.RatingCount < 0)
            return Fail(RatingSnapshotErrors.InvalidCount());

        productCardReadModel.RatingAvg = request.RatingAvg;
        productCardReadModel.RatingCount = request.RatingCount;
        productCardReadModel.UpdatedAt = timeProvider.GetUtcNow();

        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(CacheTags.ProductById(request.ProductCardId), token: cancellationToken);

        return Ok();
    }
}
