using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductSnapshotErrors
{
    public static Error NotFound() => Error.NotFound(ProductSnapshotErrorCodes.NotFound);
}

public static class ProductSnapshotErrorCodes
{
    public const string NotFound = "DefaultProductSnapshot.NotFound";
}
