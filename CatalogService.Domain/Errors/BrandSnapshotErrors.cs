using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class BrandSnapshotErrors
{
    public static Error NotFound() =>
        Error.NotFound(BrandSnapshotErrorCodes.NotFound,
            "Brand snapshot not found.");

    public static Error Inactive() =>
        Error.InvalidArgument(BrandSnapshotErrorCodes.Inactive,
            "Brand snapshot is inactive.");
}

public static class BrandSnapshotErrorCodes
{
    public const string NotFound = "BrandSnapshot.NotFound";
    public const string Inactive = "BrandSnapshot.Inactive";
}