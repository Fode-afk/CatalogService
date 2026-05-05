using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ValidateProductCardForDiscount;

public sealed record ValidateProductCardForDiscountCommand(
    Guid CorrelationId,
    Guid ProductCardId,
    Guid VendorId) : IRequest<IResult>;