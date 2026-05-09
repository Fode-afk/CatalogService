using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class BrandSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(BrandSnapshotErrorCodes.NotFound);
    public static Error Inactive() => Error.InvalidArgument(BrandSnapshotErrorCodes.Inactive);
}

public static class BrandSnapshotErrorCodes
{
    public const string NotFound = "BrandSnapshot.NotFound";
    public const string Inactive = "BrandSnapshot.Inactive";
}