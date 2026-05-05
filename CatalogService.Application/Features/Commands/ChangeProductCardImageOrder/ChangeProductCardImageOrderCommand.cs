using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ChangeProductCardImageOrder;

public sealed record ChangeProductCardImageOrderCommand(
    Guid ProductCardId,
    Guid VendorId,
    string Url,
    int NewOrder) : IRequest<IResult>;