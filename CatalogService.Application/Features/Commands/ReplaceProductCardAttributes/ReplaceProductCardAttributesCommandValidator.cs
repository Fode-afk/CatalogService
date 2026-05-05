using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ReplaceProductCardAttributes;

public sealed class ReplaceProductCardAttributesCommandValidator : AbstractValidator<ReplaceProductCardAttributesCommand>
{
    public ReplaceProductCardAttributesCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.Attributes)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductCardErrorCodes.AttributesRequired)
            .Must(x => x.Count < 31).WithErrorCode(ProductCardErrorCodes.MaxAttributesReached);
    }
}
