using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.CreateProductCard;

public sealed record CreateProductCardCommand(
    Guid VendorId,
    Guid CategoryId,

    string Name, 
    string Slug,
    string Description,
    string ShortDescription,
    string Brand,

    string SeoTitle,
    string SeoDescription,
    string SeoKeywords,

    Dictionary<string, string> Attributes,
    List<string> Tags) : IRequest<IResult>;