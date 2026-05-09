using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.UnlockProduct;

public sealed class UnlockProductCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UnlockProductCommand, IResult>
{
    public async Task<IResult> Handle(UnlockProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
           .FirstOrDefaultAsync(c => c.Id == request.ProductId, cancellationToken);
        if (product == null)
            return Fail(ProductErrors.NotFound());

        var ctx = new ProductUnlockContext(product.ProductStatus);

        var result = product.Unlock(ctx, timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
