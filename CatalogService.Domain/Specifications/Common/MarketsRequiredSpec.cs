using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

internal sealed class MarketsRequiredSpec<T> : Specification<T>
    where T : IMarketsContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (ctx.Markets.Count == 0)
            return Fail(ProductErrors.MarketsRequired());

        return Ok();
    }
}
