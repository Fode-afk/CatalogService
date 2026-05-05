using FluentValidation;

namespace CatalogService.Application.Features.Commands.ValidateProductCardForDiscount;

public sealed class ValidateProductCardForDiscountCommandValidator : AbstractValidator<ValidateProductCardForDiscountCommand>
{
    public ValidateProductCardForDiscountCommandValidator()
    {

    }
}
