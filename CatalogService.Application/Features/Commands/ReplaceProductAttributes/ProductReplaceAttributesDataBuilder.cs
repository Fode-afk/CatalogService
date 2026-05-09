using CatalogService.Domain.Errors;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CatalogService.Application.Features.Commands.ReplaceProductAttributes;

internal static class ProductReplaceAttributesDataBuilder
{
    public static IResult<List<ProductAttribute>> Build(
        ReplaceProductAttributesCommand request,
        List<CharacteristicSnapshot> characteristicSnapshots)
    {
        var errors = new List<Error>();
        var attributes = new List<ProductAttribute>();

        var snapshotMap = characteristicSnapshots
            .ToDictionary(x => x.CharacteristicId);

        foreach (var (characteristicId, value) in request.Attributes)
        {
            if (!snapshotMap.TryGetValue(characteristicId, out var snapshot))
            {
                errors.Add(CharacteristicSnapshotErrors.NotFound());
                continue;
            }

            AttributeGroupName? groupName = null;

            if (!string.IsNullOrWhiteSpace(snapshot.GroupName))
            {
                var groupResult = AttributeGroupName.Create(snapshot.GroupName);
                if (groupResult.IsFailure)
                {
                    errors.Add(groupResult.Error);
                    continue;
                }
                groupName = groupResult.Value;
            }

            var attributeResult = ProductAttribute.Create(
                snapshot.CharacteristicId,
                snapshot.Name,
                value,
                snapshot.CharType,
                groupName,
                snapshot.IsUnifying);

            if (attributeResult.IsFailure)
            {
                errors.Add(attributeResult.Error);
                continue;
            }

            attributes.Add(attributeResult.Value);
        }

        if (errors.Count > 0)
        {
            return Fail<List<ProductAttribute>>(
                Error.Validation(
                    CommonErrorCodes.ValidationFailed,
                    new Dictionary<string, object>
                    {
                        ["Errors"] = errors.Select(e => e.Code).ToList()
                    }));
        }

        return Ok(attributes);
    }
}