using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductInventorySnapshotErrors
{
    public static Error NotFound() => Error.NotFound(ProductInventorySnapshotErrorCodes.NotFound);
    public static Error OutOfStock() => Error.InvalidArgument(ProductInventorySnapshotErrorCodes.OutOfStock);
}

public static class ProductInventorySnapshotErrorCodes
{
    public const string NotFound = "ProductInventorySnapshot.NotFound";
    public const string OutOfStock = "ProductInventorySnapshot.OutOfStock";
}