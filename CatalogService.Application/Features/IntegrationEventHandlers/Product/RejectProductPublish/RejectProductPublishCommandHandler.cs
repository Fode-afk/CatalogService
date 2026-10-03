using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.RejectProductPublish;

public sealed class RejectProductPublishCommandHandle(
    IAppDbContext context,
    ICatalogMetrics metrics,
    ILogger<RejectProductPublishCommandHandle> logger,
    TimeProvider timeProvider) : IRequestHandler<RejectProductPublishCommand>
{
    public async Task Handle(RejectProductPublishCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
           .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            metrics.RecordProductNotFound();
            throw new ProductNotFoundException(request.ProductId);
        }

        var reasonResult = RejectionReason.Create(request.Reason);

        if (reasonResult.IsFailure)
        {
            logger.FailedToCreateRejectionReason(request.ProductId);
            return;
        }

        var result = product.RejectPublish(
            request.SubmissionId,
            reasonResult.Value,
            timeProvider.GetUtcNow());

        if (result.IsFailure && result.Error == ProductErrors.StaleSubmission())
        {
            logger.IgnoredStaleDecision(request.SubmissionId, request.ProductId);
            return;
        }

        if (result.IsFailure)
        {
            logger.FailedToRejectProductPublish(request.SubmissionId, request.ProductId, result.Error.Message);
            return;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
