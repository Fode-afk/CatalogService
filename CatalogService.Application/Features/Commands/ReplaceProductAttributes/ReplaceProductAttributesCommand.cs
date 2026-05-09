using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ReplaceProductAttributes;

public sealed record ReplaceProductAttributesCommand(
    Guid ProductId,
    Guid VendorId,
    Dictionary<Guid, string> Attributes) : IRequest<IResult>;