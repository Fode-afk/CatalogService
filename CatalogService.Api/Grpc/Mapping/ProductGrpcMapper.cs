using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.Application.Features.Commands.ReplaceProductTags;
using CatalogService.Application.Features.Commands.UpdateProductInfo;

namespace CatalogService.Api.Grpc.Mapping;

public static class ProductGrpcMapper
{
    public static CreateProductCommand ToCommand(this CreateProductRequest request) =>
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

    public static UpdateProductInfoCommand ToCommand(this UpdateProductInfoRequest request) =>
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

    public static ReplaceProductAttributesCommand ToCommand(this ReplaceProductAttributesRequest request) =>
        new(
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ProductId),
            request.Attributes.ToDictionary(a => Guid.Parse(a.Key), a => a.Value));

    public static ReplaceProductTagsCommand ToCommand(this ReplaceProductTagsRequest request) =>
        new(
            Guid.Parse(request.VendorId),
            Guid.Parse(request.ProductId),
            [.. request.Tags]);

    public static ProductWriteAck ToResponse(this Application.Dtos.ProductWriteAck writeAck) =>
        new()
        {
            ProductId = writeAck.ProductId.ToString(),
            Version = writeAck.Version
        };
}