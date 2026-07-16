using CatalogService.Api.Grpc.Mapping;
using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Application.Features.Commands.ArchiveProduct;
using CatalogService.Application.Features.Commands.DeleteProduct;
using CatalogService.Application.Features.Commands.LockProduct;
using CatalogService.Application.Features.Commands.PublishProduct;
using CatalogService.Application.Features.Commands.RestoreProduct;
using CatalogService.Application.Features.Commands.UnlockProduct;
using CatalogService.Application.Features.Commands.UnpublishProduct;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;

namespace CatalogService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.CatalogService.CatalogServiceBase
{
    public override async Task<Empty> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToCreateCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UpdateProductInfo(UpdateProductInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToUpdateInfoCommand(),
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

    public override async Task<Empty> UnpublishProduct(UnpublishProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new UnpublishProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> LockProduct(LockProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new LockProductCommand(Guid.Parse(request.ProductId)),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> UnlockProduct(UnlockProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new UnlockProductCommand(Guid.Parse(request.ProductId)),
            context.CancellationToken);
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

    public override async Task<Empty> RestoreProduct(RestoreProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new RestoreProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new DeleteProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductAttributes(ReplaceProductAttributesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToReplaceProductAttributesCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<Empty> ReplaceProductTags(ReplaceProductTagsRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToReplaceProductTagsCommand(),
            context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }
}