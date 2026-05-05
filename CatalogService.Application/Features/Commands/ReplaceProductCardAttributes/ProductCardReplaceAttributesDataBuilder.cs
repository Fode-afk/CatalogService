using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ReplaceProductCardAttributes;

internal static class ProductCardReplaceAttributesDataBuilder
{
    public static IResult<IReadOnlyCollection<ProductCardAttribute>> Build(ReplaceProductCardAttributesCommand request)
    {
        var errors = new List<Error>();

        var attributeResults = request.Attributes
            .Select(a => ProductCardAttribute.Create(a.Key, a.Value))
            .ToList();
        errors.AddRange(attributeResults
            .Where(r => r.IsFailure)
            .Select(r => r.Error));

        if (errors.Count > 0)
            return Fail<IReadOnlyCollection<ProductCardAttribute>>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(attributeResults.Select(a => a.Value).ToList());
    }
}
