using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.AddBrandSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.UpdateBrandSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.AddCategorySnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.DeleteCategorySnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.UpdateCategorySnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.AddCharacteristicSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.DeleteCharacteristicSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.UpdateProductVariantPriceSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.DeleteProductVariantSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;
using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;
using migApp.Shared.Dtos.ProductVariant;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.UnitTests.Fixtures;

public static class SnapshotCommandTestsFactory
{
    public static AddBrandSnapshotCommand ValidAddBrandSnapshotCommand(
        Guid? brandId = null,
        bool isAssignable = true,
        long version = 0) =>
        new(
            brandId ?? Guid.NewGuid(),
            isAssignable,
            version);

    public static UpdateBrandSnapshotCommand ValidUpdateBrandSnapshotCommand(
        Guid? brandId = null,
        bool isAssignable = true,
        long version = 1) =>
        new(
            brandId ?? Guid.NewGuid(),
            isAssignable,
            version);

    public static DeleteBrandSnapshotCommand ValidDeleteBrandSnapshotCommand(Guid? brandId = null) =>
        new(brandId ?? Guid.NewGuid());

    public static AddCategorySnapshotCommand ValidAddCategorySnapshotCommand(
        Guid? categoryId = null,
        bool isActive = true,
        long version = 0) =>
        new(
            categoryId ?? Guid.NewGuid(),
            isActive,
            version);

    public static UpdateCategorySnapshotCommand ValidUpdateCategorySnapshotCommand(
        Guid? categoryId = null,
        bool isActive = true,
        long version = 1) =>
        new(
            categoryId ?? Guid.NewGuid(),
            isActive,
            version);

    public static DeleteCategorySnapshotCommand ValidDeleteCategorySnapshotCommand(Guid? categoryId = null) =>
        new(categoryId ?? Guid.NewGuid());

    public static AddCharacteristicSnapshotCommand ValidAddCharacteristicSnapshotCommand(
        Guid? characteristicId = null,
        string name = "Color",
        AttributeCharType charType = AttributeCharType.Text,
        string? groupName = null,
        bool isUnifying = false,
        Guid? categoryId = null,
        long version = 0) =>
        new(
            characteristicId ?? Guid.NewGuid(),
            name,
            charType,
            groupName,
            isUnifying,
            categoryId ?? Guid.NewGuid(),
            version);

    public static DeleteCharacteristicSnapshotCommand ValidDeleteCharacteristicSnapshotCommand(Guid? characteristicId = null) =>
        new(characteristicId ?? Guid.NewGuid());

    public static AddProductVariantPriceSnapshotCommand ValidAddProductVariantPriceSnapshotCommand(
        Guid? productVariantId = null,
        Guid? productId = null,
        bool hasPrice = true,
        long version = 0) =>
        new(
            productVariantId ?? Guid.NewGuid(),
            productId ?? Guid.NewGuid(),
            hasPrice,
            version);

    public static UpdateProductVariantPriceSnapshotCommand ValidUpdateProductVariantPriceSnapshotCommand(
        Guid? productVariantId = null,
        bool hasPrice = true,
        long version = 1) =>
        new(
            productVariantId ?? Guid.NewGuid(),
            hasPrice,
            version);

    public static DeleteProductVariantPriceSnapshotCommand ValidDeleteProductVariantPriceSnapshotCommand(Guid? productVariantId = null) =>
        new(productVariantId ?? Guid.NewGuid());

    public static AddProductVariantSnapshotCommand ValidAddProductVariantSnapshotCommand(
        Guid? productVariantId = null,
        Guid? productId = null,
        bool hasMainImage = true,
        long version = 0,
        List<VariantAttributeDto>? characteristicValues = null) =>
        new(
            productVariantId ?? Guid.NewGuid(),
            productId ?? Guid.NewGuid(),
            hasMainImage,
            version,
            characteristicValues ?? []);

    public static VariantAttributeDto ValidVariantAttributeDto(
        Guid? characteristicId = null,
        string name = "Color",
        string value = "Red",
        AttributeCharType charType = AttributeCharType.Text,
        string? groupName = null) =>
        new(
            characteristicId ?? Guid.NewGuid(),
            name,
            value,
            charType,
            groupName);

    public static UpdateProductVariantSnapshotCommand ValidUpdateProductVariantSnapshotCommand(
        Guid? productVariantId = null,
        bool hasMainImage = true,
        long version = 1) =>
        new(
            productVariantId ?? Guid.NewGuid(),
            hasMainImage,
            version);

    public static DeleteProductVariantSnapshotCommand ValidDeleteProductVariantSnapshotCommand(Guid? productVariantId = null) =>
        new(productVariantId ?? Guid.NewGuid());

    public static AddVendorSnapshotCommand ValidAddVendorSnapshotCommand(
        Guid? vendorId = null,
        bool isActive = true,
        long version = 0) =>
        new(
            vendorId ?? Guid.NewGuid(),
            isActive,
            version);

    public static UpdateVendorSnapshotCommand ValidUpdateVendorSnapshotCommand(
        Guid? vendorId = null,
        bool isActive = true,
        long version = 1) =>
        new(
            vendorId ?? Guid.NewGuid(),
            isActive,
            version);

    public static DeleteVendorSnapshotCommand ValidDeleteVendorSnapshotCommand(Guid? vendorId = null) =>
        new(vendorId ?? Guid.NewGuid());
}
