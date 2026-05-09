using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.CreateProduct;

public sealed record CreateProductCommand(
    Guid VendorId,
    Guid CategoryId,
    Guid BrandId,

    string Name, 
    string Slug,
    string Description,
    string ShortDescription,

    string SeoTitle,
    string SeoDescription,
    string SeoKeywords) : IRequest<IResult>;