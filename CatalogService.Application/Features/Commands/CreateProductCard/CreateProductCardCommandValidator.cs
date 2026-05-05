using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.CreateProductCard;

public sealed class CreateProductCardCommandValidator : AbstractValidator<CreateProductCardCommand>
{
    public CreateProductCardCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(NameErrorCodes.NullOrEmpty)
            .MaximumLength(Name.MaxLength).WithErrorCode(NameErrorCodes.TooLong);

        RuleFor(x => x.Slug)
            .NotEmpty().WithErrorCode(SlugErrorCodes.NullOrEmpty)
            .MaximumLength(Slug.MaxLength).WithErrorCode(SlugErrorCodes.TooLong);

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

        RuleFor(x => x.Attributes)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductCardErrorCodes.AttributesRequired)
            .Must(x => x.Count <= ProductCard.MaxAttributes).WithErrorCode(ProductCardErrorCodes.MaxAttributesReached);

        RuleFor(x => x.Tags)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductCardErrorCodes.TagsRequired)
            .Must(x => x.Count <= ProductCard.MaxTags).WithErrorCode(ProductCardErrorCodes.MaxTagsReached);
    }
}
