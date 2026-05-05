using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.ChangeProductCardImageOrder;

public sealed class ChangeProductCardImageOrderCommandValidator : AbstractValidator<ChangeProductCardImageOrderCommand>
{
    public ChangeProductCardImageOrderCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.Url)
            .NotEmpty().WithErrorCode(ImageUrlErrorCodes.NullOrEmpty)
            .Must(BeValidUrl).WithErrorCode(ImageUrlErrorCodes.InvalidFormat);

        RuleFor(x => x.NewOrder)
            .LessThan(0).WithErrorCode(ProductCardImageErrorCodes.InvalidSortOrder);
    }

    private static bool BeValidUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
