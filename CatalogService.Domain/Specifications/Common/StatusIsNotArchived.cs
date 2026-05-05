using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Enums.ProductCards;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class StatusIsNotArchived<T> : Specification<T>
    where T : IStatusContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (ctx.ProductCardStatus == ProductCardStatus.Archived)
            return Fail(ProductCardErrors.CannotModify());

        return Ok();
    }
}
