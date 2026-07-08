using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductVariantSnapshotErrors
{
    public static Error ImagesRequired() =>
        Error.InvalidArgument(ProductVariantSnapshotErrorCodes.ImagesRequired,
            "Images are required.");
}

public static class ProductVariantSnapshotErrorCodes
{
    public const string ImagesRequired = "ProductVariant.ImagesRequired";
}
