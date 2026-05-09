using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

public static class ProductRestoreSpecification
{
    public static readonly ISpecification<ProductRestoreContext> Spec =
        new VendorIsActiveSpec<ProductRestoreContext>();
}
