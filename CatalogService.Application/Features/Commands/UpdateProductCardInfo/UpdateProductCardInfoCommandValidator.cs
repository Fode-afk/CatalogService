using CatalogService.Domain.Errors;
using CatalogService.Domain.ValueObjects;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.UpdateProductCardInfo;

public sealed class UpdateProductCardInfoCommandValidator : AbstractValidator<UpdateProductCardInfoCommand>
{
    public UpdateProductCardInfoCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(NameErrorCodes.NullOrEmpty)
            .MaximumLength(Name.MaxLength).WithErrorCode(NameErrorCodes.TooLong);

        RuleFor(x => x.Description)
            .NotEmpty().WithErrorCode(DescriptionErrorCodes.NullOrEmpty)
            .MaximumLength(Description.MaxLength).WithErrorCode(DescriptionErrorCodes.TooLong);

        RuleFor(x => x.ShortDescription)
            .NotEmpty().WithErrorCode(ShortDescriptionErrorCodes.NullOrEmpty)
            .MaximumLength(ShortDescription.MaxLength).WithErrorCode(ShortDescriptionErrorCodes.TooLong);

        RuleFor(x => x.Brand)
            .NotEmpty().WithErrorCode(BrandErrorCodes.NullOrEmpty)
            .MaximumLength(Brand.MaxLength).WithErrorCode(BrandErrorCodes.TooLong);

        RuleFor(x => x.SeoTitle)
            .NotEmpty().WithErrorCode(SeoTitleErrorCodes.NullOrEmpty)
            .MaximumLength(SeoTitle.MaxLength).WithErrorCode(SeoTitleErrorCodes.TooLong);

        RuleFor(x => x.SeoDescription)
            .NotEmpty().WithErrorCode(SeoDescriptionErrorCodes.NullOrEmpty)
            .MaximumLength(SeoDescription.MaxLength).WithErrorCode(SeoDescriptionErrorCodes.TooLong);

        RuleFor(x => x.SeoKeywords)
            .NotEmpty().WithErrorCode(SeoKeywordsErrorCodes.NullOrEmpty)
            .MaximumLength(SeoKeywords.MaxLength).WithErrorCode(SeoKeywordsErrorCodes.TooLong);
    }
}
