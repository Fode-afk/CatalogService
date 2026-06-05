using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class TagErrors
{
    public static Error Empty() => 
        Error.InvalidArgument(TagErrorCodes.Empty,
            "Tag is empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(TagErrorCodes.TooLong,
            $"Tag is too long. Maximum length is {maxLength} characters.");
}

public static class TagErrorCodes
{
    public const string Empty = "Tag.Empty";
    public const string TooLong = "Tag.TooLong";
}