using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class BrandErrors
{
    public static Error NullOrEmpty() => Error.InvalidArgument(BrandErrorCodes.NullOrEmpty);
    public static Error TooLong() => Error.InvalidArgument(BrandErrorCodes.TooLong);
}

public static class BrandErrorCodes
{
    public const string NullOrEmpty = "Brand.NullOrEmpty";
    public const string TooLong = "Brand.TooLong";
}
