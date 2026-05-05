using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.IncrementProductCount;

public sealed record IncrementProductCountCommand(Guid ProductCardId, Guid VendorId) : IRequest<IResult>;