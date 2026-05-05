using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UpdateProductCardInfo;

internal static class ProductCardUpdateInfoDataBuilder
{
    public static IResult<ProductCardUpdateInfoData> Build(UpdateProductCardInfoCommand request)
    {
        var errors = new List<Error>();

        var nameResult = Name.Create(request.Name);
        if (nameResult.IsFailure)
            errors.Add(nameResult.Error);

        var descriptionResult = Description.Create(request.Description);
        if (descriptionResult.IsFailure)
            errors.Add(descriptionResult.Error);

        var shortDescriptionResult = ShortDescription.Create(request.ShortDescription);
        if (shortDescriptionResult.IsFailure)
            errors.Add(shortDescriptionResult.Error);

        var brandResult = Brand.Create(request.Brand);
        if (brandResult.IsFailure)
            errors.Add(brandResult.Error);

        var seoTitleResult = SeoTitle.Create(request.SeoTitle);
        if (seoTitleResult.IsFailure)
            errors.Add(seoTitleResult.Error);

        var seoDescriptionResult = SeoDescription.Create(request.SeoDescription);
        if (seoDescriptionResult.IsFailure)
            errors.Add(seoDescriptionResult.Error);

        var seoKeywordsResult = SeoKeywords.Create(request.SeoKeywords);
        if (seoKeywordsResult.IsFailure)
            errors.Add(seoKeywordsResult.Error);

        SeoMetadata? seoMetadata = null;
        if (seoTitleResult.IsSuccess && seoDescriptionResult.IsSuccess && seoKeywordsResult.IsSuccess)
        {
            var seoMetadataResult = SeoMetadata.Create(
                seoTitleResult.Value,
                seoDescriptionResult.Value,
                seoKeywordsResult.Value);
            if (seoMetadataResult.IsFailure)
                errors.Add(seoMetadataResult.Error);
            else
                seoMetadata = seoMetadataResult.Value;
        }

        if (errors.Count > 0)
            return Fail<ProductCardUpdateInfoData>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(
            new ProductCardUpdateInfoData(
                nameResult.Value,
                descriptionResult.Value,
                shortDescriptionResult.Value,
                brandResult.Value,
                seoMetadata!));
    }
}
