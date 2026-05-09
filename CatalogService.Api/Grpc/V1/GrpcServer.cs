using migApp.Shared.Grpc;
using CatalogService.Api.Grpc.Mappers;
using CatalogService.Api.Grpc.V1.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using CatalogService.Application.Features.Queries.GetProductCardById;
using CatalogService.Application.Features.Commands.PublishProduct;
using CatalogService.Application.Features.Commands.ArchiveProduct;

namespace CatalogService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.CatalogService.CatalogServiceBase
{
    public override async Task<Empty> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToCreateCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateProductInfo(UpdateProductInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToUpdateInfoCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> PublishProduct(PublishProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new PublishProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ArchiveProduct(ArchiveProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new ArchiveProductCommand(
            Guid.Parse(request.ProductId), 
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductAttributes(ReplaceProductAttributesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToReplaceProductAttributesCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductTags(ReplaceProductTagsRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductGrpcMapper.ToReplaceProductTagsCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
           new GetProductCardByIdQuery(Guid.Parse(request.ProductId), request.CurrncyCode),
           context.CancellationToken);
        return new GetProductByIdResponse
        {
            Product = ProductGrpcMapper.FromDto(result.ThrowIfFailure())
        };
    }
}