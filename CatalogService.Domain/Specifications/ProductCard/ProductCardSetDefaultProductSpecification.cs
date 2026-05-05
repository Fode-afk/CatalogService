using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardSetDefaultProductSpecification
{
    public static readonly ISpecification<ProductCardSetDefaultProductContext> Spec =
        new VendorIsActiveSpec<ProductCardSetDefaultProductContext>()
            .And(new StatusIsNotArchived<ProductCardSetDefaultProductContext>())
            .And(Specification<ProductCardSetDefaultProductContext>.Create(
                ctx => ctx.DefaultProductBelongsToCard,
                ProductCardErrors.ProductDoesNotBelongToCard()));
}
