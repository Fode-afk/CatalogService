using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Application.Logging;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.Models;
using CatalogService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.AddProductVariantSnapshot;

public sealed class AddProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider,
    ICatalogMetrics metrics,
    ILogger<AddProductVariantSnapshotCommandHandler> logger) : IRequestHandler<AddProductVariantSnapshotCommand>
{
    public async Task Handle(AddProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductVariantSnapshots
            .AnyAsync(x => x.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (exists)
            return;

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
        {
            metrics.RecordProductNotFound();
            throw new ProductNotFoundException(request.ProductId);
        }

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == product.VendorId, cancellationToken);

        if (vendorSnapshot == null)
        {
            metrics.RecordSnapshotNotFound("Vendor");
            throw new SnapshotNotFoundException("Vendor", product.VendorId);
        }

        var characteristicIds = request.CharacteristicValues
            .Select(cv => cv.CharacteristicId)
            .ToList();

        var characteristicSnapshots = await context.CharacteristicSnapshots
            .AsNoTracking()
            .Where(c =>
                c.CategoryId == product.CategoryId &&
                characteristicIds.Contains(c.CharacteristicId))
            .ToListAsync(cancellationToken);

        var missingIds = characteristicIds
            .Except(characteristicSnapshots.Select(c => c.CharacteristicId))
            .ToList();
        if (missingIds.Count > 0)
            throw new SnapshotNotFoundException("Characteristic", missingIds.First());

        var snapshotMap = characteristicSnapshots.ToDictionary(c => c.CharacteristicId);

        var variantValues = new List<(Guid, AttributeName, AttributeCharType, AttributeGroupName?, AttributeVariableValue)>();

        foreach (var cv in request.CharacteristicValues)
        {
            var snapshot = snapshotMap[cv.CharacteristicId];

            AttributeGroupName? groupName = null;
            if (!string.IsNullOrWhiteSpace(snapshot.GroupName))
            {
                var groupResult = AttributeGroupName.Create(snapshot.GroupName);
                if (groupResult.IsFailure)
                {
                    logger.InvalidCharacteristicGroupName(snapshot.GroupName, cv.CharacteristicId, request.ProductVariantId);
                    return;
                }

                groupName = groupResult.Value;
            }

            var attributeNameResult = AttributeName.Create(snapshot.Name);
            if (attributeNameResult.IsFailure)
            {
                logger.InvalidCharacteristicName(snapshot.Name, cv.CharacteristicId, request.ProductVariantId);
                return;
            }

            variantValues.Add((
                cv.CharacteristicId,
                attributeNameResult.Value,
                snapshot.CharType,
                groupName,
                new AttributeVariableValue(cv.Value, request.ProductVariantId)));
        }

        var ctx = new ProductAddVariantToAttributesContext(
            vendorSnapshot.IsActive,
            product.CanEditContent);

        var result = product.AddVariantToAttributes(
            ctx,
            variantValues,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            logger.AddVariantToAttributesFailed(request.ProductVariantId, request.ProductId, result.Error.Message);

        context.ProductVariantSnapshots.Add(
            new Domain.Snapshots.ProductVariantSnapshot
            {
                ProductVariantId = request.ProductVariantId,
                ProductId = request.ProductId,
                HasMainImage = request.HasMainImage,
                UpdatedAt = timeProvider.GetUtcNow(),
                Version = request.Version
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}