using MediatR;

namespace CatalogService.Application.Features.IntegrationEventHandlers.Product.BlockProduct;

public sealed record BlockProductCommand(Guid ProductId, string Reason) : IRequest;