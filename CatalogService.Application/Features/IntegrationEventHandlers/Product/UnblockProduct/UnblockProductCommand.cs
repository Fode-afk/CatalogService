using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.UnblockProduct;

public sealed record UnblockProductCommand(Guid ProductId) : IRequest;