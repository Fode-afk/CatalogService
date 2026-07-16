using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.Application.Features.Commands.UpdateProductInfo;

namespace CatalogService.Api.Grpc.Mapping;

public static class ProductGrpcMapper
{
    public static CreateProductCommand ToCreateCommand(this CreateProductRequest request) =>
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

    public static UpdateProductInfoCommand ToUpdateInfoCommand(this UpdateProductInfoRequest request) =>
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

    public static ReplaceProductAttributesCommand ToReplaceProductAttributesCommand(this ReplaceProductAttributesRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductId: Guid.Parse(request.ProductId),
            Attributes: request.Attributes.ToDictionary(a => Guid.Parse(a.Key), a => a.Value));

    public static ReplaceProductTagsCommand ToReplaceProductTagsCommand(this ReplaceProductTagsRequest request) =>
        new(
            VendorId: Guid.Parse(request.VendorId),
            ProductId: Guid.Parse(request.ProductId),
            Tags: [.. request.Tags]);
}