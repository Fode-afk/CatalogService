using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UpdateProductCardInfo;

public sealed record UpdateProductCardInfoCommand(
    Guid ProductCardId,
    Guid VendorId,
    string Name,
    string Description,
    string ShortDescription,
    Guid CategoryId,
    string Brand,
    string SeoTitle,
    string SeoDescription,
    string SeoKeywords) : IRequest<IResult>;