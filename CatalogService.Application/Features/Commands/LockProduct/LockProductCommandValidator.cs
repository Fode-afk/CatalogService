using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.LockProduct;

public sealed class LockProductCommandValidator : AbstractValidator<LockProductCommand>
{
    public LockProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}