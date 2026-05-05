using migApp.Shared.Results;

namespace CatalogService.Domain.Errors;

public static class SlugErrors 
{
    public static Error NullOrEmpty() => Error.InvalidArgument(SlugErrorCodes.NullOrEmpty);
    public static Error InvalidFormat() => Error.InvalidArgument(SlugErrorCodes.InvalidFormat);
    public static Error TooLong() => Error.InvalidArgument(SlugErrorCodes.TooLong);
    public static Error TooShort() => Error.InvalidArgument(SlugErrorCodes.TooShort);
    public static Error AlreadyExists() => Error.AlreadyExists(SlugErrorCodes.AlreadyExists);
}

public static class SlugErrorCodes
{
    public const string NullOrEmpty = "Slug.NullOrEmpty";
    public const string InvalidFormat = "Slug.InvalidFormat";
    public const string TooLong = "Slug.TooLong";
    public const string TooShort = "Slug.TooShort";
    public const string AlreadyExists = "Slug.AlreadyExists";
}
