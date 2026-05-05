using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCountIncrementedSpecification
{
    public static readonly ISpecification<ProductCountIncrementedContext> Spec =
        new VendorIsActiveSpec<ProductCountIncrementedContext>()
            .And(new StatusIsNotArchived<ProductCountIncrementedContext>());
}
