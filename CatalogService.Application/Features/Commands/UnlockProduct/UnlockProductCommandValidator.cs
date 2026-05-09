using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.UnlockProduct;

public sealed class UnlockProductCommandValidator : AbstractValidator<UnlockProductCommand>
{
    public UnlockProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
         .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}