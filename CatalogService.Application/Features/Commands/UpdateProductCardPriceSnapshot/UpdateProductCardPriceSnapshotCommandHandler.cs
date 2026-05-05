using CatalogService.Application.Caching;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Services;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductCardPriceSnapshot;

public sealed class UpdateProductCardPriceSnapshotCommandHandler(
    IAppDbContext context,
    IFusionCache cache,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductCardPriceSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductCardPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductCardReadModels
            .FirstOrDefaultAsync(p => p.Id == request.ProductCardId, cancellationToken);
        if (productCardReadModel == null)
            return Fail(ProductCardErrors.NotFound());

        var priceResult = Money.Create(request.Price, Currency.USD);
        if (priceResult.IsFailure)
            return priceResult;

        Money? oldPrice = null;
        if (request.OldPrice != null)
        {
            var oldPriceResult = Money.Create(request.OldPrice.Value, Currency.USD);
            if (oldPriceResult.IsFailure)
                return oldPriceResult;

            oldPrice = oldPriceResult.Value;
        }

        if (oldPrice is not null && oldPrice < priceResult.Value)
            return Fail(ProductCardErrors.InvalidOldPrice());

        var now = timeProvider.GetUtcNow();

        productCardReadModel.PriceAmount = priceResult.Value.Amount;
        productCardReadModel.OldPriceAmount = oldPrice?.Amount;
        productCardReadModel.PriceUpdatedAt = now;
        productCardReadModel.UpdatedAt = now;

        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(CacheTags.ProductCardById(request.ProductCardId), token: cancellationToken);

        return Ok();
    }
}
