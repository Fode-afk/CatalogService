using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ReplaceProductAttributes;

public sealed class ReplaceProductAttributesCommandValidator : AbstractValidator<ReplaceProductAttributesCommand>
{
    public ReplaceProductAttributesCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.Attributes)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductErrorCodes.AttributesRequired)
            .Must(x => x.Count < 31).WithErrorCode(ProductErrorCodes.MaxAttributesReached);
    }
}