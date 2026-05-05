using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Application.Features.Commands.AddProductCardImage;
using CatalogService.Application.Features.Commands.ChangeProductCardImageOrder;
using CatalogService.Application.Features.Commands.CreateProductCard;
using CatalogService.Application.Features.Commands.RemoveProductCardImage;
using CatalogService.Application.Features.Commands.ReplaceProductCardAttributes;
using CatalogService.Application.Features.Commands.ReplaceProductCardTags;
using CatalogService.Application.Features.Commands.SetMainProductCardImage;
using CatalogService.Application.Features.Commands.UpdateProductCardImageAlt;
using CatalogService.Application.Features.Commands.UpdateProductCardInfo;

namespace CatalogService.Api.Grpc.Mappers;

public static class ProductCardGrpcMapper
{
    public static CreateProductCardCommand ToCreateCommand(CreateProductCardRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            CategoryId: Guid.Parse(request.CategoryId),
            Name: request.Name,
            Slug: request.Slug,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            Brand: request.Brand,
            SeoTitle: request.SeoTitle,
            SeoDescription: request.SeoDescription,
            SeoKeywords: request.SeoKeywords,
            Attributes: request.Attributes.ToDictionary(),
            Tags: [.. request.Tags]);

    public static UpdateProductCardInfoCommand ToUpdateInfoCommand(UpdateProductCardInfoRequest request) =>
        new(
            ProductCardId: Guid.Parse(request.ProductCardId),
            VendorId: Guid.Parse(request.VendorId),
            Name: request.Name,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            CategoryId: Guid.Parse(request.CategoryId),
            Brand: request.Brand,
            SeoTitle: request.SeoTitle,
            SeoDescription: request.SeoDescription,
            SeoKeywords: request.SeoKeywords);

    public static ReplaceProductCardAttributesCommand ToReplaceProductAttributesCommand(ReplaceProductCardAttributesRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductCardId: Guid.Parse(request.ProductCardId),
            Attributes: request.Attributes.ToDictionary());

    public static ReplaceProductCardTagsCommand ToReplaceProductTagsCommand(ReplaceProductCardTagsRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductCardId: Guid.Parse(request.ProductCardId),
            Tags: [.. request.Tags]);

    public static AddProductCardImageCommand ToAddProductImageCommand(AddProductCardImageRequest request) =>
        new(
            ProductCardId: Guid.Parse(request.ProductCardId),
            VendorId: Guid.Parse(request.VendorId),
            ImageUrl: request.Url,
            AltText: request.Alt,
            IsMain: request.IsMain);

    public static RemoveProductCardImageCommand ToRemoveProductImageCommand(RemoveProductCardImageRequest request) =>
        new(
            ProductCardId: Guid.Parse(request.ProductCardId),
            VendorId: Guid.Parse(request.VendorId),
            Url: request.Url);

    public static ChangeProductCardImageOrderCommand ToChangeProductImageOrderCommand(ChangeProductCardImageOrderRequest request) =>
        new(
            ProductCardId: Guid.Parse(request.ProductCardId),
            VendorId: Guid.Parse(request.VendorId),
            Url: request.Url,
            NewOrder: request.NewSortOrder);

    public static SetMainProductCardImageCommand ToSetMainProductImageCommand(SetMainProductCardImageRequest request) =>
        new(
            ProductCardId: Guid.Parse(request.ProductCardId),
            VendorId: Guid.Parse(request.VendorId),
            Url: request.Url);

    public static UpdateProductCardImageAltCommand ToUpdateProductImageAltCommand(UpdateProductCardImageAltRequest request) =>
        new(
            ProductCardId: Guid.Parse(request.ProductCardId),
            VendorId: Guid.Parse(request.VendorId),
            Url: request.Url,
            AltText: request.NewAlt);

    public static ProductCardDto FromDto(Application.Dtos.ProductCardDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            ShortDescription = dto.ShortDescription,
            RatingAvg = (double)dto.RatingAvg,
            RatingCount = dto.RatingCount,
            DefaultProductId = dto.DefaultProductId,
            ProductCount = dto.ProductCount,
            PriceMinorAmount = dto.PriceMinorAmount,
            OldPriceMinorAmount = dto.OldPriceMinorAmount,
            StockStatus = (StockStatus)dto.StockStatus,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            CategorySlug = dto.CategorySlug,
            VendorId = dto.VendorId,
            Brand = dto.Brand,
            ProductCardStatus = (ProductCardStatus)dto.ProductCardStatus,
            SeoMetadata = FromSeoMetadata(dto.SeoMetadata),
            Attributes = { dto.Attributes },
            Tags = { dto.Tags },
            Images = { dto.Images.Select(FromProductCardImage) }
        };

    private static SeoMetadataDto FromSeoMetadata(Application.Dtos.SeoMetadataDto dto) =>
        new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Keywords = dto.Keywords
        };

    private static ProductCardImageDto FromProductCardImage(Application.Dtos.ProductCardImageDto image) =>
        new()
        {
            Url = image.Url,
            Alt = image.Alt,
            IsMain = image.IsMain,
            SortOrder = image.SortOrder
        };
}
