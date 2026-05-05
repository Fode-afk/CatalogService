using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ProductCardSetDefaultProduct;

public sealed record ProductCardSetDefaultProductCommand(
    Guid VendorId, 
    Guid ProductCardId, 
    Guid DefaultProductId) : IRequest<IResult>;