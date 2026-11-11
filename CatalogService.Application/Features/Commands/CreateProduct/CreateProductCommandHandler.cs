using CatalogService.Application.Dtos;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IAppDbContext context,
    ICatalogMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<CreateProductCommand, IResult<ProductWriteAck>>
{
    public async Task<IResult<ProductWriteAck>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var buildResult = ProductCreationDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return Fail<ProductWriteAck>(buildResult.Error);

        var data = buildResult.Value;

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail<ProductWriteAck>(VendorSnapshotErrors.NotFound());

        var brandSnapshot = await context.BrandSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);
        if (brandSnapshot is null)
            return Fail<ProductWriteAck>(BrandSnapshotErrors.NotFound());

        var categorySnapshot = await context.CategorySnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (categorySnapshot is null)
            return Fail<ProductWriteAck>(CategorySnapshotErrors.NotFound());

        var ctx = new ProductCreationContext(
            vendorSnapshot.IsActive,
            categorySnapshot.IsActive,
            brandSnapshot.IsAssignable);

        var result = Product.Create(
            ctx,
            data,
            request.VendorId,
            request.CategoryId,
            request.BrandId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return Fail<ProductWriteAck>(result.Error);

        var product = result.Value;

        context.Products.Add(product);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Fail<ProductWriteAck>(ProductErrors.AlreadyExists());
        }

        metrics.RecordProductCreated();

        return Ok(new ProductWriteAck(product.Id, product.Version));
    }
}