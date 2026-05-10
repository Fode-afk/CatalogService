using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductVariantSnapshotErrors
{
    public static Error ImagesRequired() => Error.InvalidArgument(ProductVariantSnapshotErrorCodes.ImagesRequired);
    public static Error NotFound() => Error.NotFound(ProductVariantSnapshotErrorCodes.NotFound);
}

public static class ProductVariantSnapshotErrorCodes
{
    public const string ImagesRequired = "ProductVariant.ImagesRequired";
    public const string NotFound = "ProductVariant.NotFound";
}
