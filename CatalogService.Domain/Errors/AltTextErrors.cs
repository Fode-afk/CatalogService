using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class AltTextErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(AltTextErrorCodes.NullOrEmpty,
            "Alt text must not be empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(AltTextErrorCodes.TooLong,
            $"Alt text must not exceed {maxLength} characters.");
}

public static class AltTextErrorCodes
{
    public const string NullOrEmpty = "AltText.NullOrEmpty";
    public const string TooLong = "AltText.TooLong";
}