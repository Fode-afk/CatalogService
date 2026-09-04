using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductRemoveVariantFromAttributesSpecification
{
    public static readonly ISpecification<ProductRemoveVariantFromAttributesContext> Spec =
       new VendorIsActiveSpec<ProductRemoveVariantFromAttributesContext>()
           .And(new CanBeModifiedSpec<ProductRemoveVariantFromAttributesContext>());
}
