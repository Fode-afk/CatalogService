using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCountDecrementedSpecification
{
    public static readonly ISpecification<ProductCountDecrementedContext> Spec =
        new VendorIsActiveSpec<ProductCountDecrementedContext>()
            .And(new StatusIsNotArchived<ProductCountDecrementedContext>());
}
