using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductBlockSpecification
{
    public static readonly ISpecification<ProductBlockContext> Spec =
        Specification<ProductBlockContext>.Create(
            ctx => ctx.ProductStatus == ProductStatus.Published,
            ProductErrors.CannotBlockUnpublished());
}