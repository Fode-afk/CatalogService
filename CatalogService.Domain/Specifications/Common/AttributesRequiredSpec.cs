using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class AttributesRequiredSpec<T> : Specification<T>
    where T : IAttributesContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (ctx.Attributes.Count == 0)
            return Fail(ProductCardErrors.AttributesRequired());

        return Ok();
    }
}
