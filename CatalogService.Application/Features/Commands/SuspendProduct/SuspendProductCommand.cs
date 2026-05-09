using MediatR;
using migApp.Shared.Enums.Products;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.SuspendProduct;

public sealed record SuspendProductCommand(Guid ProductId, SuspensionReason SuspensionReason) : IRequest<IResult>;