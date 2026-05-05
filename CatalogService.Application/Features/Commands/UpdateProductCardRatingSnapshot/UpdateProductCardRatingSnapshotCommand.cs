using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UpdateProductCardRatingSnapshot;

public sealed record UpdateProductCardRatingSnapshotCommand(
    Guid ProductCardId,
    Guid VendorId,
    decimal RatingAvg,
    int RatingCount) : IRequest<IResult>;