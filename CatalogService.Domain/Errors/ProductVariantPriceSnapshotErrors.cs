using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductVariantPriceSnapshotErrors
{
    public static Error NoPrice() => Error.InvalidArgument(ProductVariantPriceSnapshotErrorCodes.NoPrice);
    public static Error NotFound() => Error.NotFound(ProductVariantPriceSnapshotErrorCodes.NotFound);
}

public static class ProductVariantPriceSnapshotErrorCodes
{
    public const string NoPrice = "ProductVariantPriceSnapshot.NoPrice";
    public const string NotFound = "ProductVariantPriceSnapshot.NotFound";
}