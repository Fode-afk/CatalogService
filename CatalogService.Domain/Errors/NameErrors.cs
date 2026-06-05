using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class NameErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(NameErrorCodes.NullOrEmpty,
            "Name cannot be null or empty.");

    public static Error TooShort(int minLength) =>
        Error.InvalidArgument(NameErrorCodes.TooShort,
            $"Name must be at least {minLength} characters long.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(NameErrorCodes.TooLong,
            $"Name must not exceed {maxLength} characters.");
}

public static class NameErrorCodes
{
    public const string NullOrEmpty = "Name.NullOrEmpty";
    public const string TooShort = "Name.TooShort";
    public const string TooLong = "Name.TooLong";

}