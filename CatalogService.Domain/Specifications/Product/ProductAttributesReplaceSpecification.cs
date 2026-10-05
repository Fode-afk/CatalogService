using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Specifications.Base;
using CatalogService.Domain.Specifications.Common;

namespace CatalogService.Domain.Specifications.Product;

internal static class ProductAttributesReplaceSpecification
{
    public static readonly ISpecification<ProductAttributesReplaceContext> Spec =
        new VendorIsActiveSpec<ProductAttributesReplaceContext>()
            .And(new CanEditContentSpec<ProductAttributesReplaceContext>())
            .And(new AttributesRequiredSpec<ProductAttributesReplaceContext>())
            .And(new AttributesLimitSpec<ProductAttributesReplaceContext>())
            .And(Specification<ProductAttributesReplaceContext>.Create(
                ctx => ctx.Attributes.Count(a => a.IsUnifying) == 1,
                ProductAttributeErrors.UnifyingAttributeRequired()));
}