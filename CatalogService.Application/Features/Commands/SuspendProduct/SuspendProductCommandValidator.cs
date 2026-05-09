using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.SuspendProduct;

public sealed class SuspendProductCommandValidator : AbstractValidator<SuspendProductCommand>
{
    public SuspendProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}
