using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.RestoreProduct;

public sealed record RestoreProductCommand(Guid ProductId, Guid VendorId) : IRequest<IResult>;