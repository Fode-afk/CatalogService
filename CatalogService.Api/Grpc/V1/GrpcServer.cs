using CatalogService.Api.Grpc.Mapping;
using CatalogService.Api.Grpc.V1.Protos;
using CatalogService.Application.Features.Commands.ArchiveProduct;
using CatalogService.Application.Features.Commands.DeleteProduct;
using CatalogService.Application.Features.Commands.RestoreProduct;
using CatalogService.Application.Features.Commands.SubmitProductForPublish;
using CatalogService.Application.Features.Commands.UnpublishProduct;
using CatalogService.Application.Features.Commands.WithdrawProductPublishSubmission;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using migApp.Shared.Grpc;

namespace CatalogService.Api.Grpc.V1;

internal sealed class GrpcServer(IMediator mediator) : Protos.CatalogService.CatalogServiceBase
{
    public override async Task<ProductWriteAck> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToCreateCommand(),
            context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> UpdateProductInfo(UpdateProductInfoRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToUpdateInfoCommand(),
            context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> SubmitProductForPublish(SubmitProductForPublishRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new SubmitProductForPublishCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> WithdrawProductPublishSubmission(WithdrawProductPublishSubmissionRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new WithdrawProductPublishSubmissionCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> UnpublishProduct(UnpublishProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new UnpublishProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> ArchiveProduct(ArchiveProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new ArchiveProductCommand(
            Guid.Parse(request.ProductId), 
            Guid.Parse(request.VendorId)), context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> RestoreProduct(RestoreProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new RestoreProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<Empty> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new DeleteProductCommand(
            Guid.Parse(request.ProductId),
            Guid.Parse(request.VendorId)), context.CancellationToken);
        result.ThrowIfFailure();
        return new Empty();
    }

    public override async Task<ProductWriteAck> ReplaceProductAttributes(ReplaceProductAttributesRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToReplaceProductAttributesCommand(),
            context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }

    public override async Task<ProductWriteAck> ReplaceProductTags(ReplaceProductTagsRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            request.ToReplaceProductTagsCommand(),
            context.CancellationToken);
        return result.ThrowIfFailure().ToGrpc();
    }
}