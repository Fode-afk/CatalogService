using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class VendorSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(VendorSnapshotErrorCodes.NotFound);
    public static Error CannotModify() => Error.Unauthenticated(VendorSnapshotErrorCodes.CannotModify);
}

public static class VendorSnapshotErrorCodes
{
    public const string NotFound = "VendorSnapshot.NotFound";
    public const string CannotModify = "VendorSnapshot.CannotModify";
}