using CatalogService.Domain.Contexts;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Products;

namespace CatalogService.TestCommon.Fixtures;

public static class ProductContextsTestFactory
{
    public static ProductCreationContext ValidCreateContext()
    {
        return new ProductCreationContext(
            VendorIsActive: true,
            CategoryIsActive: true,
            BrandIsAssignable: true);
    }

    public static ProductUpdateInfoContext ValidUpdateInfoContext()
    {
        return new ProductUpdateInfoContext(
            VendorIsActive: true,
            CanEditContent: true,
            CategoryIsActive: true,
            BrandIsAssignable: true);
    }

    public static ProductAttributesReplaceContext ValidReplaceAttributesContext(IReadOnlyCollection<ProductAttribute> attributes) =>
        new(VendorIsActive: true, CanEditContent: true, Attributes: attributes);

    public static ProductAddVariantToAttributesContext ValidAddVariantToAttributesContext() =>
        new(
            VendorIsActive: true,
            CanEditContent: true);

    public static ProductRemoveVariantFromAttributesContext ValidRemoveVariantFromAttributesContext() =>
       new(
           VendorIsActive: true,
           CanEditContent: true);

    public static ProductTagsReplaceContext ValidTagsReplaceContext(IReadOnlyCollection<Tag> tags) =>
        new(
            VendorIsActive: true,
            CanEditContent: true,
            Tags: tags);

    public static ProductSubmitForPublishContext ValidPublishContext() =>
        new(
            VendorIsActive: true,
            CategoryIsActive: true,
            BrandIsAssignable: true,
            CanEditContent: true,
            Attributes: [ProductDataTestFactory.CreateVariableAttribute(Guid.NewGuid())],
            Tags: [Tag.Create("tag").Value],
            [new ProductVariantSnapshot
            {
                ProductId = Guid.NewGuid(),
                ProductVariantId = Guid.NewGuid(),
                HasMainImage = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            }],
            [new ProductVariantPriceSnapshot
            {
                ProductId = Guid.NewGuid(),
                ProductVariantId = Guid.NewGuid(),
                HasPrice = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            }]);

    public static ProductUnpublishContext ValidUnpublishContext() =>
        new(VendorIsActive: true,
            CanEditContent: true);

    public static ProductSuspendContext ValidSuspendContext() =>
        new(CanEditContent: true);

    public static ProductArchiveContext ValidArchiveContext() =>
        new(VendorIsActive: true, CanEditContent: true);

    public static ProductRestoreContext ValidRestoreContext() =>
        new(VendorIsActive: true);

    public static ProductTryRestoreContext ValidTryRestoreContext() =>
        new(ProductStatus.Suspended);

    public static ProductBlockContext ValidLockContext() =>
        new(true, ProductStatus.Published);

    public static ProductUnblockContext ValidUnlockContext() =>
        new(ProductStatus.Published);

    public static ProductDeleteContext ValidDeleteContext() =>
            new(VendorIsActive: true, CanEditContent: true);
}
