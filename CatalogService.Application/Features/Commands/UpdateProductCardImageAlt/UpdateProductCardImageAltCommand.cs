using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UpdateProductCardImageAlt;

public sealed record UpdateProductCardImageAltCommand(
    Guid ProductCardId,
    Guid VendorId,
    string Url,
    string AltText) : IRequest<IResult>;