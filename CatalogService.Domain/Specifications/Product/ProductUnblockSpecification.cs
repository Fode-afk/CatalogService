using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductUnblockSpecification
{
    public static readonly ISpecification<ProductUnblockContext> Spec =
        Specification<ProductUnblockContext>.Create(
            ctx => ctx.IsBlocked,
            ProductErrors.CannotUnblockNotBlocked());
}