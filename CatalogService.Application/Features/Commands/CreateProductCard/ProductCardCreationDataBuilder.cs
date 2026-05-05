using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.CreateProductCard;

internal static class ProductCardCreationDataBuilder
{
    public static IResult<ProductCardCreationData> Build(CreateProductCardCommand request)
    {
        var errors = new List<Error>();

        var nameResult = Name.Create(request.Name);
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

        var attributeResults = request.Attributes
            .Select(a => ProductCardAttribute.Create(a.Key, a.Value))
            .ToList();
        errors.AddRange(attributeResults
            .Where(r => r.IsFailure)
            .Select(r => r.Error));

        var tagResults = request.Tags
            .Select(Tag.Create)
            .ToList();
        errors.AddRange(tagResults
            .Where(r => r.IsFailure)
            .Select(r => r.Error));

        if (errors.Count > 0)
            return Fail<ProductCardCreationData>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(
            new ProductCardCreationData(
                nameResult.Value,
                slugResult.Value,
                descriptionResult.Value,
                shortDescriptionResult.Value,
                brandResult.Value,
                seoMetadata!,
                [.. attributeResults.Select(r => r.Value)],
                [.. tagResults.Select(r => r.Value)]));
    }
}
