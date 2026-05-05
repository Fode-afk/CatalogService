using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class CategorySnapshotErrors
{
    public static Error NotFound() => Error.NotFound(CategorySnapshotErrorCodes.NotFound);
    public static Error Inactive() => Error.InvalidArgument(CategorySnapshotErrorCodes.Inactive);
}

public static class CategorySnapshotErrorCodes
{
    public const string NotFound = "CategorySnapshot.NotFound";
    public const string Inactive = "CategorySnapshot.Inactive";
}