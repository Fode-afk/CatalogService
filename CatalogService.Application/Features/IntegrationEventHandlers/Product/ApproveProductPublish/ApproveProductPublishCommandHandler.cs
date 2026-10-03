using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.ApproveProductPublish;

public sealed class ApproveProductPublishCommandHandler(
    IAppDbContext context,
    ICatalogMetrics metrics,
    ILogger<ApproveProductPublishCommandHandler> logger,    
    TimeProvider timeProvider) : IRequestHandler<ApproveProductPublishCommand>
{
    public async Task Handle(ApproveProductPublishCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            metrics.RecordProductNotFound();
            throw new ProductNotFoundException(request.ProductId);
        }

        var result = product.ApprovePublish(request.SubmissionId, timeProvider.GetUtcNow());

        if (result.IsFailure && result.Error == ProductErrors.StaleSubmission())
        {
            logger.IgnoredStaleDecision(request.SubmissionId, request.ProductId);
            return; 
        }

        if (result.IsFailure)
        {
            logger.FailedToApproveProductPublish(request.SubmissionId, request.ProductId, result.Error.Message);
            return;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
