using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class VariationSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(VariationSnapshotErrorCodes.NotFound);
    public static Error ImagesRequired() => Error.InvalidArgument(VariationSnapshotErrorCodes.ImagesRequired);
    public static Error VariationDoesNotBelongToCharacteristic() => Error.InvalidArgument(VariationSnapshotErrorCodes.VariationDoesNotBelongToCharacteristic);
    public static Error VariationValueMismatch() => Error.InvalidArgument(VariationSnapshotErrorCodes.VariationValueMismatch);
}

public static class VariationSnapshotErrorCodes
{
    public const string NotFound = "VariationSnapshot.NotFound";
    public const string ImagesRequired = "VariationSnapshot.ImagesRequired";
    public const string VariationDoesNotBelongToCharacteristic = "VariationSnapshot.VariationDoesNotBelongToCharacteristic";
    public const string VariationValueMismatch = "VariationSnapshot.VariationValueMismatch";
}
