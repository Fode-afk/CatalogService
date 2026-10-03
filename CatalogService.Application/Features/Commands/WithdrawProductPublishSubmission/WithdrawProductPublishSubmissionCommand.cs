using MediatR;
using migApp.Shared.Results;

namespace CatalogService.Application.Features.Commands.WithdrawProductPublishSubmission;

public sealed record WithdrawProductPublishSubmissionCommand(
    Guid ProductId, 
    Guid VendorId) : IRequest<IResult>;