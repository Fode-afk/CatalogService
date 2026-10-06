using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductSubmitForPublishSpecification
{
    public static readonly ISpecification<ProductSubmitForPublishContext> Spec =
        new VendorIsActiveSpec<ProductSubmitForPublishContext>()
            .And(new CanEditContentSpec<ProductSubmitForPublishContext>())
            .And(new BrandIsAssignableSpec<ProductSubmitForPublishContext>())
            .And(new CategoryIsActiveSpec<ProductSubmitForPublishContext>())
            .And(new AttributesRequiredSpec<ProductSubmitForPublishContext>())
            .And(new TagsRequiredSpec<ProductSubmitForPublishContext>())
            .And(Specification<ProductSubmitForPublishContext>.Create(
                ctx => ctx.PriceSnapshots.Count(p => p.HasPrice) == ctx.VariationSnapshots.Count,
                ProductVariantPriceSnapshotErrors.NoPrice()))
            .And(Specification<ProductSubmitForPublishContext>.Create(
                ctx => ctx.VariationSnapshots.All(v => v.HasMainImage),
                ProductVariantSnapshotErrors.ImagesRequired()));
}
