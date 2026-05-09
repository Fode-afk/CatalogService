using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class ProductBelongsToVendorSpec : Specification<ProductVendorOwnershipContext>
{
    public static readonly ProductBelongsToVendorSpec Instance = new();

    public override IResult IsSatisfiedBy(ProductVendorOwnershipContext ctx)
    {
        if (ctx.RequestVendorId != ctx.ProductCardVendorId)
            return Fail(ProductErrors.VendorMismatch());

        return Ok();
    }
}
