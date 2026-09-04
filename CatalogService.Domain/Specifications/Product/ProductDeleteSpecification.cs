using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductDeleteSpecification
{
    public static readonly ISpecification<ProductDeleteContext> Spec =
        new VendorIsActiveSpec<ProductDeleteContext>()
            .And(new CanBeModifiedSpec<ProductDeleteContext>());
}
