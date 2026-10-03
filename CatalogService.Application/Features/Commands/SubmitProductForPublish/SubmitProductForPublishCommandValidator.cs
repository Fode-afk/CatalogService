using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.SubmitProductForPublish;

public sealed class SubmitProductForPublishCommandValidator : AbstractValidator<SubmitProductForPublishCommand>
{
    public SubmitProductForPublishCommandValidator()
    {
        RuleFor(x => x.ProductId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}