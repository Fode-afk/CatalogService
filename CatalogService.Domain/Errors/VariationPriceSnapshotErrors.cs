using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class VariationPriceSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(VariationPriceSnapshotErrorCodes.NotFound);
    public static Error NoPrice() => Error.InvalidArgument(VariationPriceSnapshotErrorCodes.NoPrice);
}

public static class VariationPriceSnapshotErrorCodes
{
    public const string NotFound = "VariationPriceSnapshot.NotFound";
    public const string NoPrice = "VariationPriceSnapshot.NoPrice";
}