using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class CanBeModifiedSpec<T> : Specification<T>
    where T : IProductContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.CanBeModified)
            return Fail(ProductErrors.CannotModify());

        return Ok();
    }
}