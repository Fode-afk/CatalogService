using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Domain.Specifications.Common;

public sealed class ProductCardBelongsToVendorSpec : Specification<ProductCardVendorOwnershipContext>
{
    public static readonly ProductCardBelongsToVendorSpec Instance = new();

    public override IResult IsSatisfiedBy(ProductCardVendorOwnershipContext ctx) =>
        ctx.RequestVendorId == ctx.ProductCardVendorId
            ? Ok()
            : Fail(ProductCardErrors.VendorMismatch());
}
