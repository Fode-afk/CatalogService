using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ReplaceProductCardTags;

public sealed class ReplaceProductCardTagsCommandValidator : AbstractValidator<ReplaceProductCardTagsCommand>
{
    public ReplaceProductCardTagsCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
           .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
           .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId); 

        RuleFor(x => x.Tags)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductCardErrorCodes.TagsRequired)
            .Must(x => x.Count < 51).WithErrorCode(ProductCardErrorCodes.MaxTagsReached);
    }
}
