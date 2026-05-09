using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ArchiveProduct;

public sealed class ArchiveProductCommandValidator : AbstractValidator<ArchiveProductCommand>
{
    public ArchiveProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
        .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}
