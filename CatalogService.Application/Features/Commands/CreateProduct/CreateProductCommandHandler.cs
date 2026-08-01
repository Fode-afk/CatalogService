using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IAppDbContext context,
    ICatalogMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<CreateProductCommand, IResult>
{
    public async Task<IResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

        var brandSnapshot = await context.BrandSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BrandId == request.BrandId, cancellationToken);
        if (brandSnapshot is null)
            return Fail(BrandSnapshotErrors.NotFound());

        var categorySnapshot = await context.CategorySnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);
        if (categorySnapshot is null)
            return Fail(CategorySnapshotErrors.Inactive());

        var buildResult = ProductCreationDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return buildResult;

        var data = buildResult.Value;

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
            return result;

        context.Products.Add(result.Value);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Fail(ProductErrors.AlreadyExists());
        }

        metrics.RecordProductCreated();

        return Ok();
    }
}