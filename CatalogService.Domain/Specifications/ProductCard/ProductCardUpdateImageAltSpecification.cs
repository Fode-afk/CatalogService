using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardUpdateImageAltSpecification
{
    public static readonly ISpecification<ProductCardUpdateImageAltContext> Spec =
        new VendorIsActiveSpec<ProductCardUpdateImageAltContext>()
            .And(new StatusIsNotArchived<ProductCardUpdateImageAltContext>());
}
