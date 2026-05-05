using CatalogService.Application.Caching;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Services;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductCardStockSnapshot;

public sealed class UpdateProductCardStockSnapshotCommandHandler(
    IAppDbContext context,
    IFusionCache cache,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductCardStockSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductCardStockSnapshotCommand request, CancellationToken cancellationToken)
    {
        var productCardReadModel = await context.ProductCardReadModels
            .FirstOrDefaultAsync(p => p.Id == request.ProductCardId, cancellationToken);
        if (productCardReadModel == null)
            return Fail(ProductCardErrors.NotFound());

        var now = timeProvider.GetUtcNow();

        productCardReadModel.StockStatus = request.Status;
        productCardReadModel.StockUpdatedAt = now;
        productCardReadModel.UpdatedAt = now;
        
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(CacheTags.ProductCardById(request.ProductCardId), token: cancellationToken);

        return Ok();
    }
}
