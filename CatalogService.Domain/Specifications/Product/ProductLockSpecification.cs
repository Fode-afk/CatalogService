using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Specifications.Product;

public static class ProductLockSpecification
{
    public static readonly ISpecification<ProductLockContext> Spec =
        new CanBeModifiedSpec<ProductLockContext>()
            .And(Specification<ProductLockContext>.Create(
                ctx => ctx.ProductStatus == ProductStatus.Published,
                ProductErrors.CannotModify()));
}
