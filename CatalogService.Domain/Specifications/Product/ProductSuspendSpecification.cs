using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductSuspendSpecification
{
    public static readonly ISpecification<ProductSuspendContext> Spec =
        new CanEditContentSpec<ProductSuspendContext>();
}
