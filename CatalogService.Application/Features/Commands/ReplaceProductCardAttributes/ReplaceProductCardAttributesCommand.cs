using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ReplaceProductCardAttributes;

public sealed record ReplaceProductCardAttributesCommand(
    Guid ProductCardId,
    Guid VendorId,
    Dictionary<string, string> Attributes) : IRequest<IResult>;