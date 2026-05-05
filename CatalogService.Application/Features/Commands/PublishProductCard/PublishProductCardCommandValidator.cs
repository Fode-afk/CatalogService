using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.PublishProductCard;

public sealed class PublishProductCardCommandValidator : AbstractValidator<PublishProductCardCommand>
{
    public PublishProductCardCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
          .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);
    }
}
