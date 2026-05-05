using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardTagsReplacedSpecification
{
    public static readonly ISpecification<ProductCardTagsReplacedContext> Spec =
        new VendorIsActiveSpec<ProductCardTagsReplacedContext>()
            .And(new StatusIsNotArchived<ProductCardTagsReplacedContext>())
            .And(new TagsRequiredSpec<ProductCardTagsReplacedContext>())
            .And(new TagsLimitSpec<ProductCardTagsReplacedContext>());
}
