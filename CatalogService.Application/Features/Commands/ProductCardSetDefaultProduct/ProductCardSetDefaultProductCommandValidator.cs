using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ProductCardSetDefaultProduct;

public sealed class ProductCardSetDefaultProductCommandValidator : AbstractValidator<ProductCardSetDefaultProductCommand>
{
    public ProductCardSetDefaultProductCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
          .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
          .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.DefaultProductId)
          .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);
    }
}
