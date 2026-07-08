using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductVariantPriceSnapshotErrors
{
    public static Error NoPrice() =>
        Error.InvalidArgument(ProductVariantPriceSnapshotErrorCodes.NoPrice,
            "No price available.");
}

public static class ProductVariantPriceSnapshotErrorCodes
{
    public const string NoPrice = "ProductVariantPriceSnapshot.NoPrice";
}