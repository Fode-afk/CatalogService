using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

public static class ProductArchiveSpecification
{
    public static readonly ISpecification<ProductArchiveContext> Spec =
        new VendorIsActiveSpec<ProductArchiveContext>()
            .And(new CanBeModifiedSpec<ProductArchiveContext>());
}   