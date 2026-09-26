using CatalogService.Domain.Snapshots;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.TestCommon.Fixtures;

public static class SnapshotTestsFactory
{
    public static VendorSnapshot ActiveVendor(Guid? vendorId = null, long version = 0) =>
    new() { VendorId = vendorId ?? Guid.NewGuid(), IsActive = true, Version = version };

    public static VendorSnapshot InactiveVendor(Guid? vendorId = null, long version = 0) =>
        new() { VendorId = vendorId ?? Guid.NewGuid(), IsActive = false, Version = version };

    public static BrandSnapshot AssignableBrand(Guid? brandId = null, long? version = null) =>
        new() { BrandId = brandId ?? Guid.NewGuid(), IsAssignable = true, Version = version ?? 0 };

    public static CategorySnapshot ActiveCategory(Guid? categoryId = null, long? version = null) =>
        new() { CategoryId = categoryId ?? Guid.NewGuid(), IsActive = true, Version = version ?? 0 };

    public static CategorySnapshot InactiveCategory(Guid? categoryId = null, long? version = null) =>
        new() { CategoryId = categoryId ?? Guid.NewGuid(), IsActive = false, Version = version ?? 0 };

    public static CharacteristicSnapshot Characteristic(
        Guid? characteristicId = null,
        Guid? categoryId = null,
        string name = "Color",
        string? groupName = null,
        AttributeCharType charType = AttributeCharType.Text,
        bool isUnifying = false) =>
        new()
        {
            CharacteristicId = characteristicId ?? Guid.NewGuid(),
            CategoryId = categoryId ?? Guid.NewGuid(),
            Name = name,
            GroupName = groupName,
            CharType = charType,
            IsUnifying = isUnifying
        };

    public static ProductVariantPriceSnapshot PriceSnapshot(
        Guid? productId = null,
        Guid? productVariantId = null,
        bool hasPrice = true,
        long version = 0) =>
        new()
        {
            ProductId = productId ?? Guid.NewGuid(),
            ProductVariantId = productVariantId ?? Guid.NewGuid(),
            HasPrice = hasPrice,
            Version = version,
            UpdatedAt = TestClock.DefaultNow
        };

    public static ProductVariantSnapshot VariantSnapshot(
        Guid? productId = null,
        Guid? productVariantId = null,
        bool hasMainImage = true,
        long version = 0) =>
        new()
        {
            ProductId = productId ?? Guid.NewGuid(),
            ProductVariantId = productVariantId ?? Guid.NewGuid(),
            HasMainImage = hasMainImage,
            Version = version,
            UpdatedAt = TestClock.DefaultNow
        };
}
