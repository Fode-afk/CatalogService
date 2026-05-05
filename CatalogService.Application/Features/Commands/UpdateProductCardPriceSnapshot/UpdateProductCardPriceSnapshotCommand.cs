using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UpdateProductCardPriceSnapshot;

public sealed record UpdateProductCardPriceSnapshotCommand(
    Guid ProductCardId,
    Guid VendorId,
    decimal Price,
    decimal? OldPrice) : IRequest<IResult>;