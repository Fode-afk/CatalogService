using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

public static class ProductCreationSpecification
{
    public static readonly ISpecification<ProductCreationContext> Spec =
        new VendorIsActiveSpec<ProductCreationContext>()
            .And(new CategoryIsActiveSpec<ProductCreationContext>())
            .And(new BrandIsActiveSpec<ProductCreationContext>());
}