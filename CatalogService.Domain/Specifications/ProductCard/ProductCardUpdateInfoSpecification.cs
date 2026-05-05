using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardUpdateInfoSpecification
{
    public static readonly ISpecification<ProductCardUpdateInfoContext> Spec =
        new VendorIsActiveSpec<ProductCardUpdateInfoContext>()
            .And(new StatusIsNotArchived<ProductCardUpdateInfoContext>());
}
