using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ArchiveProductCard;

public sealed class ArchiveProductCardCommandValidator : AbstractValidator<ArchiveProductCardCommand>
{
    public ArchiveProductCardCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
        .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);
    }
}
