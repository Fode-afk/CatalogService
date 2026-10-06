using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.BlockProduct;

public sealed class BlockProductCommandHandler(
    IAppDbContext context,
    ICatalogMetrics metrics,
    ILogger<BlockProductCommandHandler> logger, 
    TimeProvider timeProvider) : IRequestHandler<BlockProductCommand>
{
    public async Task Handle(BlockProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(c => c.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            metrics.RecordProductNotFound();
            throw new ProductNotFoundException(request.ProductId);
        }

        var blockReasonResult = BlockReason.Create(request.Reason);

        if (blockReasonResult.IsFailure)
        {
            logger.FailedToCreateBlockReason(request.ProductId);
            return;
        }

        var ctx = new ProductBlockContext(product.ProductStatus);

        var result = product.Block(ctx, blockReasonResult.Value, timeProvider.GetUtcNow());

        if (result.IsFailure)
        {
            logger.FailedToBlockProduct(request.ProductId, result.Error.Message);
            return;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
