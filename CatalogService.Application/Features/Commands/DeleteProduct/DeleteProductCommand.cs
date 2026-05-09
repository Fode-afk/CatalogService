using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid ProductId, Guid VendorId) : IRequest<IResult>;