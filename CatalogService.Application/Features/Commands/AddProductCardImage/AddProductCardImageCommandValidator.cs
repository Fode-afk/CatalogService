using CatalogService.Domain.Errors;
using CatalogService.Domain.ValueObjects;
using FluentValidation;

namespace CatalogService.Application.Features.Commands.AddProductCardImage;

public sealed class AddProductCardImageCommandValidator : AbstractValidator<AddProductCardImageCommand>
{
    public AddProductCardImageCommandValidator()
    {
        RuleFor(x => x.ProductCardId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.VendorId)
            .NotEmpty().WithErrorCode(ProductCardErrorCodes.InvalidId);

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithErrorCode(ImageUrlErrorCodes.NullOrEmpty)
            .Must(BeValidUrl).WithErrorCode(ImageUrlErrorCodes.InvalidFormat);

        RuleFor(x => x.AltText)
            .NotEmpty().WithErrorCode(AltTextErrorCodes.NullOrEmpty)
            .MaximumLength(AltText.MaxLength).WithErrorCode(AltTextErrorCodes.TooLong);
    }

    private static bool BeValidUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
