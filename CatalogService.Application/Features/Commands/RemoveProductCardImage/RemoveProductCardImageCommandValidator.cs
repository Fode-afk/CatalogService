using CatalogService.Domain.Errors;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.RemoveProductCardImage;

public sealed class RemoveProductCardImageCommandValidator : AbstractValidator<RemoveProductCardImageCommand>
{
    public RemoveProductCardImageCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
           .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.Url)
            .NotEmpty().WithErrorCode(ImageUrlErrorCodes.NullOrEmpty)
            .Must(BeValidUrl).WithErrorCode(ImageUrlErrorCodes.InvalidFormat);
    }

    private static bool BeValidUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
             && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
