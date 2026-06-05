using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductVariantSnapshotErrors
{
    public static Error ImagesRequired() =>
        Error.InvalidArgument(ProductVariantSnapshotErrorCodes.ImagesRequired,
            "Images are required.");
    public static Error NotFound() =>
        Error.NotFound(ProductVariantSnapshotErrorCodes.NotFound,
            "Product variant snapshot not found.");
}

public static class ProductVariantSnapshotErrorCodes
{
    public const string ImagesRequired = "ProductVariant.ImagesRequired";
    public const string NotFound = "ProductVariant.NotFound";
}
