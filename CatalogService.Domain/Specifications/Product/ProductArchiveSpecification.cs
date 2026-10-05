using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductArchiveSpecification
{
    public static readonly ISpecification<ProductArchiveContext> Spec =
        new VendorIsActiveSpec<ProductArchiveContext>()
            .And(new CanEditContentSpec<ProductArchiveContext>());
}   