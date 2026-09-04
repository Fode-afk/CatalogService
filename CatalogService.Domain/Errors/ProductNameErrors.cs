using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ProductNameErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(ProductNameErrorCodes.NullOrEmpty,
            "Product name cannot be null or empty.");

    public static Error TooShort(int minLength) =>
        Error.InvalidArgument(ProductNameErrorCodes.TooShort,
            $"Product name must be at least {minLength} characters long.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(ProductNameErrorCodes.TooLong,
            $"Product name must not exceed {maxLength} characters.");
}

public static class ProductNameErrorCodes
{
    public const string NullOrEmpty = "ProductName.NullOrEmpty";
    public const string TooShort = "ProductName.TooShort";
    public const string TooLong = "ProductName.TooLong";

}