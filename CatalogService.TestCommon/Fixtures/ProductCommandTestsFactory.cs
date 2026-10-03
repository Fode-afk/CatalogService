using CatalogService.Application.Features.Commands.ArchiveProduct;
using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Features.Commands.DeleteProduct;
using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.Application.Features.Commands.RestoreProduct;
using CatalogService.Application.Features.Commands.SubmitProductForPublish;
using CatalogService.Application.Features.Commands.UnpublishProduct;
using CatalogService.Application.Features.Commands.UpdateProductInfo;
using CatalogService.Application.Features.IntegrationEventHandlers.Product.BlockProduct;
using CatalogService.Application.Features.IntegrationEventHandlers.Product.UnblockProduct;

namespace CatalogService.TestCommon.Fixtures;

public static class ProductCommandTestsFactory
{
    public static CreateProductCommand ValidCreateCommand(
        Guid? vendorId = null, Guid? categoryId = null, Guid? brandId = null) =>
        new(
            vendorId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid(),
            brandId ?? Guid.NewGuid(),
            "Name", "slug", "Description", "Short",
            "Seo title", "Seo desc", "keyword");

    public static UpdateProductInfoCommand ValidUpdateInfoCommand(
        Guid? productId = null,
        Guid? vendorId = null,
        Guid? categoryId = null,
        Guid? brandId = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid(),
            "Name", "slug", "Description", "Short",
            categoryId ?? Guid.NewGuid(),
            brandId ?? Guid.NewGuid(),
            "Seo title", "Seo desc", "keyword");

    public static ReplaceProductAttributesCommand ValidReplaceAttributesCommand(
        Guid? productId = null,
        Guid? vendorId = null,
        Dictionary<Guid, string>? attributes = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid(),
            attributes ?? new Dictionary<Guid, string> { [Guid.NewGuid()] = "Red" });

    public static ReplaceProductTagsCommand ValidReplaceTagsCommand(
        Guid? productId = null,
        Guid? vendorId = null,
        List<string>? tags = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid(),
            tags ?? ["tag"]);

    public static SubmitProductForPublishCommand ValidPublishCommand(
        Guid? productId = null,
        Guid? vendorId = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid());

    public static UnpublishProductCommand ValidUnpublishCommand(
        Guid? productId = null,
        Guid? vendorId = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid());

    public static BlockProductCommand ValidLockCommand(Guid? productId = null) =>
        new(productId ?? Guid.NewGuid());

    public static UnblockProductCommand ValidUnlockCommand(Guid? productId = null) =>
        new(productId ?? Guid.NewGuid());

    public static ArchiveProductCommand ValidArchiveCommand(
        Guid? productId = null,
        Guid? vendorId = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid());

    public static RestoreProductCommand ValidRestoreCommand(
        Guid? productId = null,
        Guid? vendorId = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid());

    public static DeleteProductCommand ValidDeleteCommand(
        Guid? productId = null,
        Guid? vendorId = null) =>
        new(
            productId ?? Guid.NewGuid(),
            vendorId ?? Guid.NewGuid());
}
