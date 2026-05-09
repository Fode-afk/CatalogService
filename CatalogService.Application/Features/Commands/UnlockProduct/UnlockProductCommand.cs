using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.UnlockProduct;

public sealed record UnlockProductCommand(Guid ProductId) : IRequest<IResult>;