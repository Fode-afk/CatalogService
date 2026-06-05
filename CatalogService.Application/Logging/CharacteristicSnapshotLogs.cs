using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Logging;

public static partial class CharacteristicSnapshotLogs
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Error,
        Message = "Invalid GroupName '{GroupName}' in CharacteristicSnapshot {CharacteristicId}. " +
                  "Skipping variant snapshot creation for ProductVariant {VariantId}.")]
    public static partial void InvalidCharacteristicGroupName(
        this ILogger logger, string groupName, Guid characteristicId, Guid variantId);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Error,
        Message = "Invalid Name '{Name}' in CharacteristicSnapshot {CharacteristicId}. " +
                  "Skipping variant snapshot creation for ProductVariant {VariantId}.")]
    public static partial void InvalidCharacteristicName(
        this ILogger logger, string name, Guid characteristicId, Guid variantId);
}
