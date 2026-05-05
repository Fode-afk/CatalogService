using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardChangeImageOrderSpecification
{
    public static readonly ISpecification<ProductCardChangeImageOrderContext> Spec =
        new VendorIsActiveSpec<ProductCardChangeImageOrderContext>()
            .And(new StatusIsNotArchived<ProductCardChangeImageOrderContext>());
}
