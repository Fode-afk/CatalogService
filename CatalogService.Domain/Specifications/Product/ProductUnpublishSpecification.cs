using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductUnpublishSpecification
{
    public static readonly ISpecification<ProductUnpublishContext> Spec =
        new VendorIsActiveSpec<ProductUnpublishContext>()
            .And(Specification<ProductUnpublishContext>.Create(
                ctx => ctx.ProductStatus == ProductStatus.Published,
                ProductErrors.CannotUnpublishNotPublished()));
}