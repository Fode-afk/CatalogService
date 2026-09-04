using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.CreateProduct;

internal static class ProductCreationDataBuilder
{
    public static IResult<ProductCreationData> Build(CreateProductCommand request)
    {
        var errors = new List<Error>();

        var nameResult = ProductName.Create(request.Name);
        if (nameResult.IsFailure)
            errors.Add(nameResult.Error);

        var slugResult = Slug.Create(request.Slug);
        if (slugResult.IsFailure)
            errors.Add(slugResult.Error);

        var descriptionResult = Description.Create(request.Description);
        if (descriptionResult.IsFailure)
            errors.Add(descriptionResult.Error);

        var shortDescriptionResult = ShortDescription.Create(request.ShortDescription);
        if (shortDescriptionResult.IsFailure)
            errors.Add(shortDescriptionResult.Error);

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
            return Fail<ProductCreationData>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(
            new ProductCreationData(
                nameResult.Value,
                slugResult.Value,
                descriptionResult.Value,
                shortDescriptionResult.Value,
                seoMetadata!));
    }
}