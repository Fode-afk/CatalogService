using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardSetMainImageSpecification
{
    public static readonly ISpecification<ProductCardSetMainImageContext> Spec =
        new VendorIsActiveSpec<ProductCardSetMainImageContext>()
            .And(new StatusIsNotArchived<ProductCardSetMainImageContext>());
}
