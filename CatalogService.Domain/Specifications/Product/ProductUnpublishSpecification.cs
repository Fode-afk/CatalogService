using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductUnpublishSpecification
{
    public static readonly ISpecification<ProductUnpublishContext> Spec =
        new VendorIsActiveSpec<ProductUnpublishContext>()
            .And(new CanEditContentSpec<ProductUnpublishContext>());
}