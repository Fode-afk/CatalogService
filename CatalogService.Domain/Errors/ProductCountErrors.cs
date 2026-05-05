using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductCountErrors
{
    public static Error InvalidValue() => Error.InvalidArgument(ProductCountErrorCodes.InvalidValue);
    public static Error LimitReached() => Error.InvalidArgument(ProductCountErrorCodes.LimitReached);
}

public static class ProductCountErrorCodes
{
    public const string InvalidValue = "ProductCount.InvalidValue";
    public const string LimitReached = "ProductCount.LimitReached";
}