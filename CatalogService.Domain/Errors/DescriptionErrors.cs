using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class DescriptionErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(DescriptionErrorCodes.NullOrEmpty,
            "Description cannot be null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(DescriptionErrorCodes.TooLong,
            $"Description must not exceed {maxLength} characters.");
}

public static class DescriptionErrorCodes
{
    public const string NullOrEmpty = "Description.NullOrEmpty";
    public const string TooLong = "Description.TooLong";
}