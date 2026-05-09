using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.LockProduct;

public sealed record LockProductCommand(Guid ProductId) : IRequest<IResult>;