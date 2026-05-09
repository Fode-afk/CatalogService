using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Contexts;
using CatalogService.Domain.Enums;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.AddProductVariantSnapshot;

public sealed class AddProductVariantSnapshotCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductVariantSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(AddProductVariantSnapshotCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.ProductVariantSnapshots
            .AnyAsync(x => x.ProductVariantId == request.ProductVariantId, cancellationToken);
        if (exists)
            return Ok();

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product is null)
            return Fail(ProductErrors.NotFound());

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == product.VendorId, cancellationToken);
        if (vendorSnapshot is null)
            return Fail(VendorSnapshotErrors.NotFound());

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
            return Fail(CharacteristicSnapshotErrors.NotFound());

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
                    return groupResult;

                groupName = groupResult.Value;
            }

            var attributeNameResult = AttributeName.Create(snapshot.Name);
            if (attributeNameResult.IsFailure)
                return attributeNameResult;

            variantValues.Add((
                cv.CharacteristicId,
                attributeNameResult.Value,
                snapshot.CharType,
                groupName,
                new AttributeVariableValue(cv.Value, request.ProductVariantId)));
        }

        var ctx = new ProductAddVariantToAttributesContext(
            vendorSnapshot.IsActive,
            product.CanBeModified);

        var result = product.AddVariantToAttributes(
            ctx,
            variantValues,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        context.ProductVariantSnapshots.Add(new ProductVariantSnapshot
        {
            ProductVariantId = request.ProductVariantId,
            ProductId = request.ProductId,
            HasMainImage = request.HasMainImage,
            UpdatedAt = timeProvider.GetUtcNow(),
            Version = 1
        });

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
