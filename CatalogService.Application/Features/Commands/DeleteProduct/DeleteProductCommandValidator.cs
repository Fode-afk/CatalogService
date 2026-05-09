using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.DeleteProduct;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);
    }
}