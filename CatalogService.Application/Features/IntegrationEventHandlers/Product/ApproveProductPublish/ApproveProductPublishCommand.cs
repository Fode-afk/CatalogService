using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.ApproveProductPublish;

public sealed record ApproveProductPublishCommand(Guid ProductId, Guid SubmissionId) : IRequest;
