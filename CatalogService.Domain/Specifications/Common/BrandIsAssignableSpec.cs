using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class BrandIsAssignableSpec<T> : Specification<T>
    where T : IBrandContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.BrandIsAssignable)
            return Fail(BrandSnapshotErrors.Inactive());

        return Ok();
    }
}
