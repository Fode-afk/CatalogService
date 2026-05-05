using migApp.Shared.Grpc;
using CatalogService.Api.Grpc.Mappers;
using CatalogService.Api.Grpc.V1.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using CatalogService.Application.Features.Commands.ArchiveProductCard;
using CatalogService.Application.Features.Commands.PublishProductCard;
using CatalogService.Application.Features.Queries.GetProductCardById;

namespace CatalogService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.CatalogService.CatalogServiceBase
{
    public override async Task<Empty> CreateProductCard(CreateProductCardRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToCreateCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateProductCardInfo(UpdateProductCardInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToUpdateInfoCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> PublishProductCard(PublishProductCardRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new PublishProductCardCommand(
            Guid.Parse(request.ProductCardId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ArchiveProductCard(ArchiveProductCardRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new ArchiveProductCardCommand(
            Guid.Parse(request.ProductCardId), 
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductCardAttributes(ReplaceProductCardAttributesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToReplaceProductAttributesCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductCardTags(ReplaceProductCardTagsRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToReplaceProductTagsCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> AddProductCardImage(AddProductCardImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToAddProductImageCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> RemoveProductCardImage(RemoveProductCardImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToRemoveProductImageCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ChangeProductCardImageOrder(ChangeProductCardImageOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToChangeProductImageOrderCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> SetMainProductCardImage(SetMainProductCardImageRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToSetMainProductImageCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateProductCardImageAlt(UpdateProductCardImageAltRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            ProductCardGrpcMapper.ToUpdateProductImageAltCommand(request),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<GetProductCardByIdResponse> GetProductCardById(GetProductCardByIdRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
           new GetProductCardByIdQuery(Guid.Parse(request.ProductCardId), request.CurrncyCode),
           context.CancellationToken);
        return new GetProductCardByIdResponse
        {
            ProductCard = ProductCardGrpcMapper.FromDto(result.ThrowIfFailure())
        };
    }
}