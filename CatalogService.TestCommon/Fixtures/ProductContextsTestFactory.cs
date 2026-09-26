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
            CanBeModified: true,
            CategoryIsActive: true,
            BrandIsAssignable: true);
    }

    public static ProductAttributesReplaceContext ValidReplaceAttributesContext(IReadOnlyCollection<ProductAttribute> attributes) =>
        new(VendorIsActive: true, CanBeModified: true, Attributes: attributes);

    public static ProductAddVariantToAttributesContext ValidAddVariantToAttributesContext() =>
        new(
            VendorIsActive: true,
            CanBeModified: true);

    public static ProductRemoveVariantFromAttributesContext ValidRemoveVariantFromAttributesContext() =>
       new(
           VendorIsActive: true,
           CanBeModified: true);

    public static ProductTagsReplaceContext ValidTagsReplaceContext(IReadOnlyCollection<Tag> tags) =>
        new(
            VendorIsActive: true,
            CanBeModified: true,
            Tags: tags);

    public static ProductPublishContext ValidPublishContext() =>
        new(
            VendorIsActive: true,
            CategoryIsActive: true,
            BrandIsAssignable: true,
            CanBeModified: true,
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
            CanBeModified: true);

    public static ProductSuspendContext ValidSuspendContext() =>
        new(CanBeModified: true);

    public static ProductArchiveContext ValidArchiveContext() =>
        new(VendorIsActive: true, CanBeModified: true);

    public static ProductRestoreContext ValidRestoreContext() =>
        new(VendorIsActive: true);

    public static ProductTryRestoreContext ValidTryRestoreContext() =>
        new(ProductStatus.Suspended);

    public static ProductLockContext ValidLockContext() =>
        new(true, ProductStatus.Published);

    public static ProductUnlockContext ValidUnlockContext() =>
        new(ProductStatus.Published);

    public static ProductDeleteContext ValidDeleteContext() =>
            new(VendorIsActive: true, CanBeModified: true);
}
