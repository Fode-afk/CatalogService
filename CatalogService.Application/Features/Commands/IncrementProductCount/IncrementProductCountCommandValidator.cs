using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.IncrementProductCount;

public sealed class IncrementProductCountCommandValidator : AbstractValidator<IncrementProductCountCommand>
{
    public IncrementProductCountCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);
    }
}