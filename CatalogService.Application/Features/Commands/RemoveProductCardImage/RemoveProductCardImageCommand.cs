using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.RemoveProductCardImage;

public sealed record RemoveProductCardImageCommand(
    Guid ProductCardId,
    Guid VendorId,
    string Url) : IRequest<IResult>;