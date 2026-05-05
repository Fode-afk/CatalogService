using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardRemoveImageSpecification
{
    public static readonly ISpecification<ProductCardRemoveImageContext> Spec =
        new VendorIsActiveSpec<ProductCardRemoveImageContext>()
            .And(new StatusIsNotArchived<ProductCardRemoveImageContext>());
}
