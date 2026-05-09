using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ReplaceProductTags;

public sealed class ReplaceProductTagsCommandValidator : AbstractValidator<ReplaceProductTagsCommand>
{
    public ReplaceProductTagsCommandValidator()
    {
        RuleFor(x => x.ProductId)
           .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
           .NotEmpty().WithErrorCode(ProductErrorCodes.InvalidId); 

        RuleFor(x => x.Tags)
            .NotNull()
            .Must(x => x.Count > 0).WithErrorCode(ProductErrorCodes.TagsRequired)
            .Must(x => x.Count < 51).WithErrorCode(ProductErrorCodes.MaxTagsReached);
    }
}
