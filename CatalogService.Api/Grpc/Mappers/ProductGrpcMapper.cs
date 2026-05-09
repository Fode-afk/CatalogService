using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.Application.Features.Commands.UpdateProductInfo;

namespace CatalogService.Api.Grpc.Mappers;

public static class ProductGrpcMapper
{
    public static CreateProductCommand ToCreateCommand(CreateProductRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            CategoryId: Guid.Parse(request.CategoryId),
            BrandId: Guid.Parse(request.BrandId),
            Name: request.Name,
            Slug: request.Slug,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            SeoTitle: request.SeoTitle,
            SeoDescription: request.SeoDescription,
            SeoKeywords: request.SeoKeywords);

    public static UpdateProductInfoCommand ToUpdateInfoCommand(UpdateProductInfoRequest request) =>
        new(
            ProductId: Guid.Parse(request.ProductId),
            VendorId: Guid.Parse(request.VendorId),
            Name: request.Name,
            Slug: request.Slug,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            CategoryId: Guid.Parse(request.CategoryId),
            BrandId: Guid.Parse(request.BrandId),
            SeoTitle: request.SeoTitle,
            SeoDescription: request.SeoDescription,
            SeoKeywords: request.SeoKeywords);

    public static ReplaceProductAttributesCommand ToReplaceProductAttributesCommand(ReplaceProductAttributesRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductId: Guid.Parse(request.ProductId),
            Attributes: request.Attributes.ToDictionary(a => Guid.Parse(a.Key), a => a.Value));

    public static ReplaceProductTagsCommand ToReplaceProductTagsCommand(ReplaceProductTagsRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductId: Guid.Parse(request.ProductId),
            Tags: [.. request.Tags]);

    public static ProductDto FromDto(Application.Dtos.ProductDto dto) =>
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
            Tags = { dto.Tags }
        };

    private static SeoMetadataDto FromSeoMetadata(Application.Dtos.SeoMetadataDto dto) =>
        new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Keywords = dto.Keywords
        };
}
