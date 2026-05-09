using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UnpublishProduct;

public sealed record UnpublishProductCommand(Guid ProductId, Guid VendorId) : IRequest<IResult>;