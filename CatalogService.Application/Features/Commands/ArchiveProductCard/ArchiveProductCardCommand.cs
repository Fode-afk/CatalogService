using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ArchiveProductCard;

public sealed record ArchiveProductCardCommand(Guid ProductCardId, Guid VendorId) : IRequest<IResult>;