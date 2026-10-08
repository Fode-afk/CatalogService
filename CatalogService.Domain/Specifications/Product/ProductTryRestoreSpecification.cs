using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductTryRestoreSpecification
{
    public static readonly ISpecification<ProductTryRestoreContext> Spec =
        Specification<ProductTryRestoreContext>.Create(
            ctx => ctx.IsActive,
            ProductErrors.Inactive());
}