using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductBlockSpecification
{
    public static readonly ISpecification<ProductBlockContext> Spec =
        new CanBeModifiedSpec<ProductBlockContext>()
            .And(Specification<ProductBlockContext>.Create(
                ctx => ctx.ProductStatus == ProductStatus.Published,
                ProductErrors.CannotModify()));
}
