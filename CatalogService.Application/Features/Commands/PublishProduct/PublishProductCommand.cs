using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.PublishProduct;

public sealed record PublishProductCommand(Guid ProductId, Guid VendorId) : IRequest<IResult>;