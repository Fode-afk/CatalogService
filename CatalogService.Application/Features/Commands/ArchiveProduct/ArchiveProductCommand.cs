using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.ArchiveProduct;

public sealed record ArchiveProductCommand(Guid ProductId, Guid VendorId) : IRequest<IResult>;