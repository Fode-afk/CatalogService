using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

public static class ProductAddVariantToAttributesSpecification
{
    public static readonly ISpecification<ProductAddVariantToAttributesContext> Spec =
        new VendorIsActiveSpec<ProductAddVariantToAttributesContext>()
            .And(new CanBeModifiedSpec<ProductAddVariantToAttributesContext>());
}
