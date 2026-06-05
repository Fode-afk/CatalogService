using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductVariantPriceSnapshotErrors
{
    public static Error NoPrice() =>
        Error.InvalidArgument(ProductVariantPriceSnapshotErrorCodes.NoPrice,
            "No price available.");
    public static Error NotFound() =>
        Error.NotFound(ProductVariantPriceSnapshotErrorCodes.NotFound,
            "Product variant price snapshot not found.");
}

public static class ProductVariantPriceSnapshotErrorCodes
{
    public const string NoPrice = "ProductVariantPriceSnapshot.NoPrice";
    public const string NotFound = "ProductVariantPriceSnapshot.NotFound";
}