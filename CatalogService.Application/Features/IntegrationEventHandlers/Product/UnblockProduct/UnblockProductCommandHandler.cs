using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.UnblockProduct;

public sealed class UnblockProductCommandHandler(
    IAppDbContext context,
    ICatalogMetrics metrics,
    ILogger<UnblockProductCommandHandler> logger,
    TimeProvider timeProvider) : IRequestHandler<UnblockProductCommand>
{
    public async Task Handle(UnblockProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
           .FirstOrDefaultAsync(c => c.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            metrics.RecordProductNotFound();
            throw new ProductNotFoundException(request.ProductId);
        }

        var ctx = new ProductUnblockContext(product.IsBlocked);

        var result = product.Unblock(ctx, timeProvider.GetUtcNow());

        if (result.IsFailure)
        {
            logger.FailedToUnblockProduct(request.ProductId, result.Error.Message);
            return;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
