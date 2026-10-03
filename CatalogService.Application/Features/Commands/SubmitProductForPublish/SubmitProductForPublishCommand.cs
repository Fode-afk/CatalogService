using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.SubmitProductForPublish;

public sealed record SubmitProductForPublishCommand(Guid ProductId, Guid VendorId) : IRequest<IResult>;