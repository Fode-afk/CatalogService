using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.DecrementProductCount;

public sealed record DecrementProductCountCommand(Guid ProductCardId, Guid VendorId) : IRequest<IResult>;