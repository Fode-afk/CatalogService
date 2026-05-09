using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.SuspendProduct;

public sealed class SuspendProductCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<SuspendProductCommand, IResult>
{
    public async Task<IResult> Handle(SuspendProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
          .FirstOrDefaultAsync(c => c.Id == request.ProductId, cancellationToken);
        if (product == null)
            return Fail(ProductErrors.NotFound());

        var ctx = new ProductSuspendContext(product.CanBeModified);

        var result = product.Suspend(ctx, new(request.SuspensionReason), timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
