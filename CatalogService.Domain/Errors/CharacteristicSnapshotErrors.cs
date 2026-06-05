using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class CharacteristicSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(CharacteristicSnapshotErrorCodes.NotFound,
            "Characteristic snapshot not found.");
}

public static class CharacteristicSnapshotErrorCodes
{
    public const string NotFound = "CharacteristicSnapshot.NotFound";
}