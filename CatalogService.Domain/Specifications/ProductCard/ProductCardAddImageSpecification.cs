using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardAddImageSpecification
{
    public static readonly ISpecification<ProductCardAddImageContext> Spec =
        new VendorIsActiveSpec<ProductCardAddImageContext>()
            .And(new StatusIsNotArchived<ProductCardAddImageContext>())
            .And(Specification<ProductCardAddImageContext>.Create(
                ctx => ctx.ImagesCount < Models.ProductCard.MaxImages,
                ProductCardImageErrors.MaxImagesReached()));
}
