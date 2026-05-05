using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.AddProductCardImage;

public static class AddProductCardImageDataBuilder
{
    public static IResult<AddProductCardImageData> Build(AddProductCardImageCommand request)
    {
        var errors = new List<Error>();

        var imageUrlResult = ImageUrl.Create(request.ImageUrl);
        if (imageUrlResult.IsFailure)
            errors.Add(imageUrlResult.Error);

        var altTextResult = AltText.Create(request.AltText);
        if (altTextResult.IsFailure)
            errors.Add(altTextResult.Error);

        if (errors.Count > 0)
            return Fail<AddProductCardImageData>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(
            new AddProductCardImageData(
                imageUrlResult.Value,
                altTextResult.Value));
    }
}
