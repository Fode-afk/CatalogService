using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ReplaceProductCardTags;

public sealed record ReplaceProductCardTagsCommand(
    Guid ProductCardId,
    Guid VendorId,
    List<string> Tags) : IRequest<IResult>;