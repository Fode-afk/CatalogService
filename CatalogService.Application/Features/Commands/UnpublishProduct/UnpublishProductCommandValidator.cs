using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.UnpublishProduct;

public sealed class UnpublishProductCommandValidator : AbstractValidator<UnpublishProductCommand>
{
    public UnpublishProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}