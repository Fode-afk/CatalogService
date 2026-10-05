using CatalogService.Domain.Contexts;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductTagsReplaceSpecification
{
    public static readonly ISpecification<ProductTagsReplaceContext> Spec =
        new VendorIsActiveSpec<ProductTagsReplaceContext>()
            .And(new CanEditContentSpec<ProductTagsReplaceContext>())
            .And(new TagsRequiredSpec<ProductTagsReplaceContext>())
            .And(new TagsLimitSpec<ProductTagsReplaceContext>());
}
