using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Enums.Products;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.DeleteProductVariantPriceSnapshot;

public sealed class DeleteProductVariantPriceSnapshotCommandHandler(IAppDbContext context) : IRequestHandler<DeleteProductVariantPriceSnapshotCommand>
{
    public async Task Handle(DeleteProductVariantPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await context.ProductVariantPriceSnapshots
           .FirstOrDefaultAsync(p => p.PriceId == request.PriceId, cancellationToken);

        if (snapshot is null)
            return;

        context.ProductVariantPriceSnapshots.Remove(snapshot);

        await context.SaveChangesAsync(cancellationToken);
    }
}