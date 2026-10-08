using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductSuspendSpecification
{
    public static readonly ISpecification<ProductSuspendContext> Spec =
        Specification<ProductSuspendContext>.Create(
            ctx => ctx.CanEditOperationalData,
            ProductErrors.CannotEditOperationalData());
}
