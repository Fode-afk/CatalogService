using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.DecrementProductCount;

public sealed class DecrementProductCountCommandValidator : AbstractValidator<DecrementProductCountCommand>
{
    public DecrementProductCountCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
          .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);
    }
}