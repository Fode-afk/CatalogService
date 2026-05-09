using CatalogService.Application.Dtos;
using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Queries.GetProductCardById;

public sealed record GetProductCardByIdQuery(
    Guid ProductCardId,
    string Currency) : IRequest<IResult<ProductDto>>;