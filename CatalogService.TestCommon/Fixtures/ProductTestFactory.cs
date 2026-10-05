using CatalogService.Domain.Contexts;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Characteristics;
using migApp.Shared.Enums.Products;

namespace CatalogService.TestCommon.Fixtures;

public static class ProductTestFactory
{
    public static Product CreateValid(
        Guid? vendorId = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        DateTimeOffset? now = null,
        string suffix = "")
    {
        var context = new ProductCreationContext(
            VendorIsActive: true,
            CategoryIsActive: true,
            BrandIsAssignable: true);

        var data = ProductDataTestFactory.CreateData(suffix);

        var product = Product.Create(
            context,
            data,
            vendorId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid(),
            brandId ?? Guid.NewGuid(),
            now ?? TestClock.DefaultNow).Value;

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreatePublished(
        Guid? variantId = null,
        Guid? vendorId = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        string suffix = "")
    {
        var product = CreateValid(vendorId: vendorId, brandId: brandId, categoryId: categoryId, suffix: suffix);
        var productVariantId = variantId ?? Guid.NewGuid();

        product.ReplaceTags(ProductContextsTestFactory.ValidTagsReplaceContext([Tag.Create("tag").Value]), TestClock.DefaultNow);

        var variant = ProductDataTestFactory.CreateVariantValue(
            characteristicId: Guid.NewGuid(),
            value: "Blue",
            valueId: productVariantId);

        product.AddVariantToAttributes(ProductContextsTestFactory.ValidAddVariantToAttributesContext(), [variant], TestClock.DefaultNow);

        product.SubmitForPublish(new ProductSubmitForPublishContext(
            VendorIsActive: true,
            CategoryIsActive: true,
            BrandIsAssignable: true,
            CanEditContent: true,
            product.Attributes,
            product.Tags,
            [new ProductVariantSnapshot
            { 
                ProductId = product.Id,
                ProductVariantId = productVariantId,
                HasMainImage = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            }],
            [new ProductVariantPriceSnapshot
            {
                ProductId = product.Id,
                ProductVariantId = productVariantId,
                HasPrice = true,
                Version = 0,
                UpdatedAt = TestClock.DefaultNow
            }]), TestClock.DefaultNow);

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreateSuspended(
        Guid? brandId = null, 
        Guid? categoryId = null,
        Guid? vendorId = null,
        string suffix = "",
        params ProductSuspensionReason[] suspensionReasons)
    {
        var product = CreatePublished(brandId: brandId, categoryId: categoryId, vendorId: vendorId, suffix: suffix);

        if (suspensionReasons.Length == 0)
        {
            suspensionReasons =
            [
                new ProductSuspensionReason(
                SuspensionReason.VendorDeactivated)
            ];
        }

        foreach (var reason in suspensionReasons)
        {
            product.Suspend(
                ProductContextsTestFactory.ValidSuspendContext(),
                reason,
                TestClock.DefaultNow);
        }

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreateArchived()
    {
        var product = CreateValid();

        product.Archive(
            ProductContextsTestFactory.ValidArchiveContext(),
            TestClock.DefaultNow);

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreateLockedByAdmin()
    {
        var product = CreatePublished();

        product.Block(ProductContextsTestFactory.ValidLockContext(), TestClock.DefaultNow);

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreateDeleted(Guid? vendorId = null)
    {
        var product = CreateValid(vendorId: vendorId);

        product.Delete(
            ProductContextsTestFactory.ValidDeleteContext(),
            TestClock.DefaultNow);

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreateWithVariableAttribute(Guid variantId, Guid? variantId2 = null)
    {
        var product = CreateValid();

        var ctx = ProductContextsTestFactory.ValidAddVariantToAttributesContext();

        var characteristicId = Guid.NewGuid();

        var variantValues = new List<(
            Guid CharacteristicId,
            AttributeName Name,
            AttributeCharType CharType,
            AttributeGroupName? GroupName,
            AttributeVariableValue VariableValue)>
        {
            ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "Red",
                valueId: variantId)
        };

        if (variantId2 != null)
            variantValues.Add(ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "White",
                valueId: variantId2));

        product.AddVariantToAttributes(
            ctx,
            variantValues,
            TestClock.DefaultNow);

        product.ClearDomainEvents();

        return product;
    }

    public static Product CreateWithAttributes()
    {
        var product = CreateValid();

        var attributes = new List<ProductAttribute> { ProductDataTestFactory.CreateAttribute(Guid.NewGuid(), isUnifying: true) };
        product.ReplaceAttributes(ProductContextsTestFactory.ValidReplaceAttributesContext(attributes), TestClock.DefaultNow);

        var ctx = ProductContextsTestFactory.ValidAddVariantToAttributesContext();

        var characteristicId = Guid.NewGuid();

        var variantValues = new List<(
            Guid CharacteristicId,
            AttributeName Name,
            AttributeCharType CharType,
            AttributeGroupName? GroupName,
            AttributeVariableValue VariableValue)>
        {
            ProductDataTestFactory.CreateVariantValue(
                characteristicId: characteristicId,
                value: "Red",
                valueId: Guid.NewGuid())
        };

        product.AddVariantToAttributes(
            ctx,
            variantValues,
            TestClock.DefaultNow);

        return product;
    }

    public static Product CreateWithTags(Tag[] tags)
    {
        var product = CreateValid();

        product.ReplaceTags(
            ProductContextsTestFactory.ValidTagsReplaceContext(tags),
            TestClock.DefaultNow);

        product.ClearDomainEvents();

        return product;
    }
}