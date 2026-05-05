using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.ProductCard;

public static class ProductCardPublishSpecification
{
    public static readonly ISpecification<ProductCardPublishContext> Spec =
        new VendorIsActiveSpec<ProductCardPublishContext>()
            .And(new StatusIsNotArchived<ProductCardPublishContext>())
            .And(Specification<ProductCardPublishContext>.Create(
                ctx => ctx.HasDefaultProduct,
                ProductCardErrors.NoDefaultProduct()))
            .And(Specification<ProductCardPublishContext>.Create(
                ctx => ctx.DefaultProductHasPrice,
                ProductPriceSnapshotErrors.NoPrice()))
            .And(Specification<ProductCardPublishContext>.Create(
                ctx => ctx.DefaultProductInStock,
                ProductInventorySnapshotErrors.OutOfStock()))
            .And(Specification<ProductCardPublishContext>.Create(
                ctx => !ctx.ProductCount.IsEmpty,
                ProductCardErrors.NoVariants()))
            .And(Specification<ProductCardPublishContext>.Create(
                ctx => ctx.Images.Count != 0,
                ProductCardErrors.ImagesRequired()));
}
