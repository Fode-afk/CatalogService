using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductTryRestoreSpecification
{
    public static readonly ISpecification<ProductTryRestoreContext> Spec =
        Specification<ProductTryRestoreContext>.Create(
            ctx => ctx.ProductStatus != ProductStatus.Archived,
            ProductErrors.CannotEditContent());
}
