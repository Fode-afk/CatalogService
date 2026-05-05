using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class TagsLimitSpec<T> : Specification<T>
    where T : ITagsContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (ctx.Tags.Count > Models.ProductCard.MaxTags)
            return Fail(ProductCardErrors.MaxTagsReached());

        return Ok();
    }
}