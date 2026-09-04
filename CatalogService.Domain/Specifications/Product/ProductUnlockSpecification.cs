using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductUnlockSpecification
{
    public static readonly ISpecification<ProductUnlockContext> Spec =
        Specification<ProductUnlockContext>.Create(
            ctx => ctx.ProductStatus != ProductStatus.Archived,
            ProductErrors.CannotModify());
}