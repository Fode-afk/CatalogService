using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardAttributesReplacedSpecification
{
    public static readonly ISpecification<ProductCardAttributesReplacedContext> Spec =
        new VendorIsActiveSpec<ProductCardAttributesReplacedContext>()
            .And(new StatusIsNotArchived<ProductCardAttributesReplacedContext>())
            .And(new AttributesRequiredSpec<ProductCardAttributesReplacedContext>())
            .And(new AttributesLimitSpec<ProductCardAttributesReplacedContext>());
}
