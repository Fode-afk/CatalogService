using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.WithdrawProductPublishSubmission;

public sealed class WithdrawProductPublishSubmissionCommandValidator : AbstractValidator<WithdrawProductPublishSubmissionCommand>
{
    public WithdrawProductPublishSubmissionCommandValidator()
    {
        RuleFor(x => x.ProductId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}
