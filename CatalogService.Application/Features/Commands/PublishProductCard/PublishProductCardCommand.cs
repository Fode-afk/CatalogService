using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.PublishProductCard;

public sealed record PublishProductCardCommand(Guid ProductCardId, Guid VendorId) : IRequest<IResult>;