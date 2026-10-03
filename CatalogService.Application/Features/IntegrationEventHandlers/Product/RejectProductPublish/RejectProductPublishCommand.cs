using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.RejectProductPublish;

public sealed record RejectProductPublishCommand(
    Guid ProductId, 
    Guid SubmissionId,
    string Reason) : IRequest;