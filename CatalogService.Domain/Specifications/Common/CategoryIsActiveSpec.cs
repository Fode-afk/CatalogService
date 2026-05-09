using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class CategoryIsActiveSpec<T> : Specification<T>
    where T : ICategoryContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.CategoryIsActive)
            return Fail(CategorySnapshotErrors.Inactive());

        return Ok();
    }
}
