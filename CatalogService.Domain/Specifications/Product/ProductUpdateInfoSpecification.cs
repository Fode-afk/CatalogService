using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductUpdateInfoSpecification
{
    public static readonly ISpecification<ProductUpdateInfoContext> Spec =
        new VendorIsActiveSpec<ProductUpdateInfoContext>()
            .And(new CanEditContentSpec<ProductUpdateInfoContext>())
            .And(new CategoryIsActiveSpec<ProductUpdateInfoContext>())
            .And(new BrandIsAssignableSpec<ProductUpdateInfoContext>());
}