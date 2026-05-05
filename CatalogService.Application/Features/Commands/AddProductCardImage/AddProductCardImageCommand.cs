using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.AddProductCardImage;

public sealed record AddProductCardImageCommand(
    Guid ProductCardId, 
    Guid VendorId,
    string ImageUrl,
    string AltText,
    bool IsMain) : IRequest<IResult>;