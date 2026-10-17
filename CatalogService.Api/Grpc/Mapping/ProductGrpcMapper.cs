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
            Guid.Parse(request.VendorId),
            Guid.Parse(request.CategoryId),
            Guid.Parse(request.BrandId),
            request.Name,
            request.Slug,
            request.Description,
            request.ShortDescription,
            request.SeoTitle,
            request.SeoDescription,
            request.SeoKeywords);

    public static UpdateProductInfoCommand ToUpdateInfoCommand(this UpdateProductInfoRequest request) =>
        new(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId),
            request.Name,
            request.Slug,
            request.Description,
            request.ShortDescription,
            Guid.Parse(request.CategoryId),
            Guid.Parse(request.BrandId),
            request.SeoTitle,
            request.SeoDescription,
            request.SeoKeywords);

    public static ReplaceProductAttributesCommand ToReplaceProductAttributesCommand(this ReplaceProductAttributesRequest request) =>
        new(
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ProductId),
            request.Attributes.ToDictionary(a => Guid.Parse(a.Key), a => a.Value));

    public static ReplaceProductTagsCommand ToReplaceProductTagsCommand(this ReplaceProductTagsRequest request) =>
        new(
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ProductId),
            [.. request.Tags]);
}