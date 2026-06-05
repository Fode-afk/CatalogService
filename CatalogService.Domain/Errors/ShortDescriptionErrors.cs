using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class ShortDescriptionErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(ShortDescriptionErrorCodes.NullOrEmpty,
            "Short description is null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(ShortDescriptionErrorCodes.TooLong,
            $"Short description is too long. Maximum length is {maxLength} characters.");
}

public static class ShortDescriptionErrorCodes
{
    public const string NullOrEmpty = "ShortDescription.NullOrEmpty";
    public const string TooLong = "ShortDescription.TooLong";
}