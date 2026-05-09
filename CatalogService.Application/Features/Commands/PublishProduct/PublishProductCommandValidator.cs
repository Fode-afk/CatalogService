using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.PublishProduct;

public sealed class PublishProductCommandValidator : AbstractValidator<PublishProductCommand>
{
    public PublishProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}
