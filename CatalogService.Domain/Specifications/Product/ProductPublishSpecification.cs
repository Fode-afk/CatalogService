using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

public static class ProductPublishSpecification
{
    public static readonly ISpecification<ProductPublishContext> Spec =
        new VendorIsActiveSpec<ProductPublishContext>()
            .And(new CanBeModifiedSpec<ProductPublishContext>())
            .And(new BrandIsActiveSpec<ProductPublishContext>())
            .And(new CategoryIsActiveSpec<ProductPublishContext>())
            .And(new AttributesRequiredSpec<ProductPublishContext>())
            .And(new TagsRequiredSpec<ProductPublishContext>())
            .And(Specification<ProductPublishContext>.Create(
                ctx => ctx.PriceSnapshots.Count(p => p.HasPrice) == ctx.VariationSnapshots.Count,
                VariationPriceSnapshotErrors.NoPrice()))
            .And(Specification<ProductPublishContext>.Create(
                ctx => ctx.VariationSnapshots.All(v => v.HasMainImage),
                VariationSnapshotErrors.ImagesRequired()));
}
