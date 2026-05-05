using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductPriceSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(ProductPriceSnapshotErrorCodes.NotFound);
    public static Error NoPrice() => Error.InvalidArgument(ProductPriceSnapshotErrorCodes.NoPrice);
}

public static class ProductPriceSnapshotErrorCodes
{
    public const string NotFound = "ProductPriceSnapshot.NotFound";
    public const string NoPrice = "ProductPriceSnapshot.NoPrice";
}