using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class CharacteristicSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(CharacteristicSnapshotErrorCodes.NotFound);
}

public static class CharacteristicSnapshotErrorCodes
{
    public const string NotFound = "CharacteristicSnapshot.NotFound";
}