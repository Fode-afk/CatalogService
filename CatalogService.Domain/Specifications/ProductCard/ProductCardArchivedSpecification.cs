using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardArchivedSpecification
{
    public static readonly ISpecification<ProductCardArchivedContext> Spec =
        new VendorIsActiveSpec<ProductCardArchivedContext>();
}
