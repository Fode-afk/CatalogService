using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UpdateProductInfo;

public sealed record UpdateProductInfoCommand(
    Guid ProductId,
    Guid VendorId,

    string Name,
    string Slug,
    string Description,
    string ShortDescription,

    Guid CategoryId,
    Guid BrandId,

    string SeoTitle,
    string SeoDescription,
    string SeoKeywords) : IRequest<IResult>;