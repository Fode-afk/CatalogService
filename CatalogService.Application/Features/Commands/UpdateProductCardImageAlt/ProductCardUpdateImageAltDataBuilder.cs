using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductCardImageAlt;

public static class ProductCardUpdateImageAltDataBuilder
{
    public static IResult<ProductCardUpdateImageAltData> Build(UpdateProductCardImageAltCommand request)
    {
        var errors = new List<Error>();

        var altTextResult = AltText.Create(request.AltText);
        if (altTextResult.IsFailure)
            errors.Add(altTextResult.Error);

        var imageUrlResult = ImageUrl.Create(request.Url);
        if (imageUrlResult.IsFailure)
            errors.Add(imageUrlResult.Error);

        if (errors.Count > 0)
            return Fail<ProductCardUpdateImageAltData>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(
            new ProductCardUpdateImageAltData(
                imageUrlResult.Value,
                altTextResult.Value));
    }
}
