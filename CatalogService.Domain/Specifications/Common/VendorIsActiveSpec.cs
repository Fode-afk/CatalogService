using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class VendorIsActiveSpec<T> : Specification<T>
    where T : IVendorContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.VendorIsActive)
            return Fail(VendorSnapshotErrors.CannotModify());

        return Ok();
    }
}