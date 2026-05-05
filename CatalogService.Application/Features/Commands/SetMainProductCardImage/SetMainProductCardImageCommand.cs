using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.SetMainProductCardImage;

public sealed record SetMainProductCardImageCommand(
    Guid ProductCardId,
    Guid VendorId,
    string Url) : IRequest<IResult>;