using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ReplaceProductCardTags;

public static class ProductCardTagsReplacedDataBuilder
{
    public static IResult<IReadOnlyCollection<Tag>> Build(ReplaceProductCardTagsCommand request)
    {
        var errors = new List<Error>();

        var tagResults = request.Tags
            .Select(Tag.Create)
            .ToList();
        errors.AddRange(tagResults
            .Where(r => r.IsFailure)
            .Select(r => r.Error));

        if (errors.Count > 0)
            return Fail<IReadOnlyCollection<Tag>>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(tagResults.Select(a => a.Value).ToList());
    }
}
