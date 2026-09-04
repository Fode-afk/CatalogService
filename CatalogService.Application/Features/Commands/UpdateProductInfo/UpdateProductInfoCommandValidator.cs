using CatalogService.Domain.Errors;
using CatalogService.Domain.ValueObjects;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.UpdateProductInfo;

public sealed class UpdateProductInfoCommandValidator : AbstractValidator<UpdateProductInfoCommand>
{
    public UpdateProductInfoCommandValidator()
    {
        RuleFor(x => x.ProductId)
           .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.BrandId)
          .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(ProductNameErrorCodes.NullOrEmpty)
            .MaximumLength(ProductName.MaxLength).WithErrorCode(ProductNameErrorCodes.TooLong);

        RuleFor(x => x.Slug)
            .NotEmpty().WithErrorCode(SlugErrorCodes.NullOrEmpty)
            .MaximumLength(Slug.MaxLength).WithErrorCode(SlugErrorCodes.TooLong);

        RuleFor(x => x.Description)
            .NotEmpty().WithErrorCode(DescriptionErrorCodes.NullOrEmpty)
            .MaximumLength(Description.MaxLength).WithErrorCode(DescriptionErrorCodes.TooLong);

        RuleFor(x => x.ShortDescription)
            .NotEmpty().WithErrorCode(ShortDescriptionErrorCodes.NullOrEmpty)
            .MaximumLength(ShortDescription.MaxLength).WithErrorCode(ShortDescriptionErrorCodes.TooLong);

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
