using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ReplaceProductTags;

public sealed record ReplaceProductTagsCommand(
    Guid ProductId,
    Guid VendorId,
    List<string> Tags) : IRequest<IResult>;