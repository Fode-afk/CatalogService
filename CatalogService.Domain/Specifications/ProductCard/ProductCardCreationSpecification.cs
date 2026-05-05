using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;
namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardCreationSpecification
{
    public static readonly ISpecification<ProductCardCreationContext> Spec =
        new VendorIsActiveSpec<ProductCardCreationContext>()
            .And(Specification<ProductCardCreationContext>.Create(
                ctx => ctx.CategoryIsActive,
                CategorySnapshotErrors.Inactive()))
            .And(new AttributesRequiredSpec<ProductCardCreationContext>())
            .And(new AttributesLimitSpec<ProductCardCreationContext>())
            .And(new TagsRequiredSpec<ProductCardCreationContext>())
            .And(new TagsLimitSpec<ProductCardCreationContext>());
}
