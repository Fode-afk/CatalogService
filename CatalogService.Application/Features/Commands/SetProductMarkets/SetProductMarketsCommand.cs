using CatalogService.Application.Dtos;
using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.SetProductMarkets;

public sealed record SetProductMarketsCommand(
    Guid ProductId,
    Guid VendorId,
    List<string> Markets) : IRequest<IResult<ProductWriteAck>>;